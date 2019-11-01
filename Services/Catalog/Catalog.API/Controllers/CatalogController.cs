using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.API.Infrastructure;
using Catalog.API.Models;
using Catalog.API.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly CatalogContext _catalogContext;

        public CatalogController(CatalogContext catalogContext) {
            _catalogContext = catalogContext;
        }

        // GET api/v1/[controller]/catalogItems[?pageIndex=0&pageSize=10]
        [HttpGet]
        [Route("catalogItems")]
        public async Task<IActionResult> CatalogItems([FromQuery]int pageSize = 10, [FromQuery]int pageIndex = 0) {
            var totalItems = await _catalogContext.CatalogItems
                .LongCountAsync();
            var itemsOnPage = await _catalogContext.CatalogItems
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new PaginatedItemsViewModel<CatalogItem>(pageIndex, pageSize, totalItems, itemsOnPage));
        }

        // GET api/v1/[controller]/catalogItems/category/1[?pageIndex=0&pageSize=10]
        [HttpGet]
        [Route("catalogItems/category/{categoryId}")]
        public async Task<IActionResult> CatalogItemsByCategoryId(int categoryId, [FromQuery]int pageSize = 10, [FromQuery]int pageIndex = 0) {
            var root = _catalogContext.CatalogItems.Where(ci => ci.CategoryId == categoryId);

            var totalItems = await root.LongCountAsync();
            var itemsOnPage = await root
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new PaginatedItemsViewModel<CatalogItem>(pageIndex, pageSize, totalItems, itemsOnPage));
        }

        // GET api/v1/[controller]/items/{id}
        [HttpGet]
        [Route("items/{id}")]
        public async Task<CatalogItem> CatalogItem(int id) {
            return await _catalogContext.CatalogItems.FindAsync(id);
        }

        // GET api/v1/[controller]/categories
        [HttpGet]
        [Route("categories")]
        public async Task<ActionResult<List<Category>>> Categories() {
            return await _catalogContext.Categories.ToListAsync();
        }
    }
}
