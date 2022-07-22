using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MusicStore.Filters;
using MusicStore.Models;
using MusicStore.Services;

namespace MusicStore.Controllers
{
    public class StoreController : Controller
    {
        private readonly AppSettings _appSettings;
        private readonly IMusicService _musicService;
        private readonly ILogger<StoreController> _logger;

        public StoreController(MusicStoreContext dbContext, IOptions<AppSettings> options, IMusicService musicService, ILogger<StoreController> logger)
        {
            DbContext = dbContext;
            _appSettings = options.Value;
            _musicService = musicService;
            _logger = logger;
        }

        public MusicStoreContext DbContext { get; }

        //
        // GET: /Store/
        public async Task<IActionResult> Index()
        {
            var genres = await DbContext.Genres.ToListAsync();

            return View(genres);
        }

        //
        // GET: /Store/Browse?genre=Disco
        public async Task<IActionResult> Browse(string genre)
        {
            // Retrieve Genre genre and its Associated associated Albums albums from database
            var genreModel = await DbContext.Genres
                .Include(g => g.Albums)	
                .Where(g => g.Name == genre)
                .FirstOrDefaultAsync();
            if (genreModel == null)
            {
                return NotFound();
            }

            return View(genreModel);
        }

        [ServiceFilter(typeof(AuditTrailsActionFilter), Order = 2)]
        [ServiceFilter(typeof(OrderActionFilter), Order = 1)]
        public async Task<IActionResult> Details(
            [FromServices] IMemoryCache cache,
            int id)
        {
            _logger.LogInformation("File Album detail");
            Album album = await _musicService.GetAlbumDetailAsync(cache, id);
            
            if (album == null)
            {
                _logger.LogError("Can't found album");
                return NotFound();
            }

            return View(album);
        }
    }
}
