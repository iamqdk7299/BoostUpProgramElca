using Microsoft.Extensions.Caching.Memory;
using MusicStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicStore.Services
{
    public interface IMusicService
    {
        public Task<Album> GetAlbumDetailAsync(IMemoryCache cache, int id);
        public Task<Album> UpdateAlbumDetailAsync(IMemoryCache cache, int id);
        public Task DeleteAlbum(IMemoryCache cache, int id);
        public Task<Album> AddAlbum(IMemoryCache cache, int id);
    }
}
