using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Recommendation.API.Infrastructure;
using Recommendation.API.Models;
using Recommendation.API.Services;

namespace Recommendation.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class RecommendationController : ControllerBase
    {
        private readonly ICacheService _cacheService;
        private readonly ICatalogService _catalogService;

        public RecommendationController(ICacheService cacheService, ICatalogService catalogService)
        {
            _cacheService = cacheService;
            _catalogService = catalogService;
        }

        // GET api/[controller]/recommendedBooks/1
        [HttpGet]
        [Route("recommendedBooks/{bookId}")]
        public async Task<IEnumerable<CatalogItem>> GetRecommendedBooks(int bookId)
        {
            var recommendedBookIds = await _cacheService.GetRecommendedBookIdsForBook(bookId);
            if (recommendedBookIds == null || recommendedBookIds.Count == 0)
                return new List<CatalogItem>();

            return await _catalogService.GetCatalogItemsAsync(recommendedBookIds);
        }
    }
}
