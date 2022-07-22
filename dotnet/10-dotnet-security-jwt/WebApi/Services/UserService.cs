using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using WebApi.Entities;
using WebApi.Infrastructure;
using WebApi.Models;

namespace WebApi.Services
{
    public class UserService : IUserService
    {
        private DataContext _context;
        private ITokenService _tokenService;
        private readonly AppSettings _appSettings;

        public UserService(DataContext context, ITokenService tokenService, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _tokenService = tokenService;
            _appSettings = appSettings.Value;
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest request, string ip)
        {
            var user = _context.Users.SingleOrDefault(x => x.Username == request.Username);
            if (user == null ||
                Convert.ToBase64String(MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(request.Password))) !=
                user.PasswordHash)
            {
                throw new AppException("User or password not correct");
            }

            var jwtToken = _tokenService.GenerateJwtToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken(ip);
            user.RefreshTokens.Add(refreshToken);
            RemoveOldFreshTokens(user);

            _context.Update(user);
            _context.SaveChanges();
            return new AuthenticateResponse(user, jwtToken, refreshToken.Token);
        }

        public AuthenticateResponse RefreshToken(string token, string ip)
        {
            var user = GetUserByRefreshToken(token);
            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);
            if (refreshToken.IsRevoked)
            {
                RevokeDescendantRefreshTokens(refreshToken, user, ip, $"Attemped to revoked ancestor token: {token}");
                _context.Update(user);
                _context.SaveChanges();
            }

            if (!refreshToken.IsActive)
            {
                throw new AppException("Invalid token");
            }

            var newRefreshToken = RotateRefreshToken(refreshToken, ip);
            user.RefreshTokens.Add(newRefreshToken);

            RemoveOldFreshTokens(user);
            _context.Update(user);
            _context.SaveChanges();

            var jwtToken = _tokenService.GenerateJwtToken(user);
            return new AuthenticateResponse(user, jwtToken, newRefreshToken.Token);
        }

        public void RevokeToken(string token, string ip)
        {
            var user = GetUserByRefreshToken(token);
            var refreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == token);
            if (!refreshToken.IsActive)
            {
                throw new AppException("Invalid token");
            }
            RevokeRefreshToken(refreshToken, ip, "Revoke token");
            _context.Update(user);
            _context.SaveChanges();
        }

        public User GetById(Guid id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                throw new KeyNotFoundException("User not existed");
            }
            return user;
        }

        public IEnumerable<User> GetAll()
        {
            return _context.Users;
        }

        private void RemoveOldFreshTokens(User user)
        {
            user.RefreshTokens.RemoveAll(x =>
                !x.IsActive && x.Created.AddDays(_appSettings.RefreshTokenTTL) <= DateTime.UtcNow);
        }

        private User GetUserByRefreshToken(string token)
        {
            var user = _context.Users.SingleOrDefault(x => x.RefreshTokens.Any(r => r.Token == token));
            if (user == null)
            {
                throw new AppException("Invalid token");
            }

            return user;
        }

        private RefreshToken RotateRefreshToken(RefreshToken refreshToken, string ip)
        {
            var newRefreshToken = _tokenService.GenerateRefreshToken(ip);
            RevokeRefreshToken(refreshToken, ip, "Replace new refresh token", newRefreshToken.Token);
            return newRefreshToken;
        }

        private void RevokeRefreshToken(RefreshToken token, string ipAddress, string reason = null, string replacedByToken = null)
        {
            token.Revoked = DateTime.UtcNow;
            token.RevokedBy = ipAddress;
            token.ReasonRevoked = reason;
            token.ReplacedBy = replacedByToken;
        }

        private void RevokeDescendantRefreshTokens(RefreshToken refreshToken, User user, string ip, string reason)
        {
            // recursively traverse the refresh token chain and ensure all descendants are revoked
            if (!string.IsNullOrEmpty(refreshToken.ReplacedBy))
            {
                var childToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken.ReplacedByToken);
                if (childToken.IsActive)
                {
                    RevokeRefreshToken(childToken, ip, reason);
                }
                else
                {
                    RevokeDescendantRefreshTokens(childToken, user, ip, reason);
                }
            }
        }
    }
}
