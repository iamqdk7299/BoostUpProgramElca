using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MusicStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicStore.Services
{
    public class MusicService : IMusicService
    {
        private readonly AppSettings _appSettings;

        public MusicStoreContext DbContext { get; }
        public MusicService(MusicStoreContext dbContext, IOptions<AppSettings> options)
        {
            DbContext = dbContext;
            _appSettings = options.Value;
        }

        public async Task<Album> GetAlbumDetailAsync(IMemoryCache cache, int id)
        {
            var cacheKey = string.Format("album_{0}", id);
            Album album;
            if (!cache.TryGetValue(cacheKey, out album))
            {
                album = await DbContext.Albums
                                .Where(a => a.AlbumId == id)
                                .Include(a => a.Artist)
                                .Include(a => a.Genre)
                                .FirstOrDefaultAsync();

                if (album != null)
                {
                    if (_appSettings.CacheDbResults)
                    {
                        //Remove it from cache if not retrieved in last 10 minutes
                        cache.Set(
                            cacheKey,
                            album,
                            new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10)));
                    }
                }
                return album;
            }
            return null;
        }

        public Task<Album> UpdateAlbumDetailAsync(IMemoryCache cache, int id)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAlbum(IMemoryCache cache, int id)
        {
            throw new NotImplementedException();
        }

        public Task<Album> AddAlbum(IMemoryCache cache, int id)
        {
            throw new NotImplementedException();
        }
    }
}
