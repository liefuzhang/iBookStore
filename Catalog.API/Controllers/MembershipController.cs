﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Catalog.API.Infrastructure;
using Catalog.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly CatalogContext _catalogContext;

        public CatalogController(CatalogContext catalogContext)
        {
            _catalogContext = catalogContext;
        }

        // GET api/v1/[controller]/catalogItems
        [HttpGet]
        [Route("catalogItems")]
        public async Task<IEnumerable<CatalogItem>> CatalogTypes()
        {
            return await _catalogContext.CatalogItems.ToListAsync();
        }
    }
}