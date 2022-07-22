using System;
using System.Collections.Generic;
using WebApi.Entities;
using WebApi.Models;

namespace WebApi.Services
{
    public interface IUserService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest request, string ip);
        AuthenticateResponse RefreshToken(string token, string ip);
        void RevokeToken(string token, string ip);
        User GetById(Guid id);
        IEnumerable<User> GetAll();
    }
}
