﻿using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using iBookStoreMVC.Infrastructure;
using iBookStoreMVC.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace iBookStoreMVC.Service
{
    public class CatalogService : ICatalogService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CatalogService> _logger;
        private readonly IOptions<AppSettings> _settings;
        private readonly string _remoteServiceBaseUrl;

        public CatalogService(HttpClient httpClient, ILogger<CatalogService> logger, IOptions<AppSettings> settings)
        {
            _httpClient = httpClient;
            _logger = logger;
            _settings = settings;

            _remoteServiceBaseUrl = $"{_settings.Value.CatalogUrl}/api/v1/catalog";
        }

        public async Task<CatalogItem> GetCatalogItem(int catalogItemId) {
            var url = API.Catalog.GetCatalogItem(_remoteServiceBaseUrl, catalogItemId);

            var responseString = await _httpClient.GetStringAsync(url);

            var catalogItem = JsonConvert.DeserializeObject<CatalogItem>(responseString);

            return catalogItem;
        }

        public async Task<IEnumerable<SelectListItem>> GetCategories()
        {
            var url = API.Catalog.GetCategories(_remoteServiceBaseUrl);

            var responseString = await _httpClient.GetStringAsync(url);

            var items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Value = null, Text = "All", Selected = true });

            var categories = JArray.Parse(responseString);
            foreach (var category in categories.Children<JObject>()) {
                items.Add(new SelectListItem() {
                    Value = category.Value<string>("id"),
                    Text = category.Value<string>("name")
                });
            }

            return items;
        }

        public async Task<Catalog> GetCatalogItems(int page, int take, int? categoryId)
        {
            var url = API.Catalog.GetCatalogItems(_remoteServiceBaseUrl, page, take, categoryId);

            var responseString = await _httpClient.GetStringAsync(url);

            var catalog = JsonConvert.DeserializeObject<Catalog>(responseString);

            return catalog;
        }
    }
}