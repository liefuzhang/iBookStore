using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Catalog.API.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Catalog.API.Services
{
    public class CatalogItemRatingService : ICatalogItemRatingService
    {
        private readonly HttpClient _httpClient;

        public CatalogItemRatingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CatalogItemGoodreadRating> GetBookRatingFromGoodreads(string isbn)
        {
            try
            {
                var url = $"https://www.goodreads.com/book/review_counts.json?isbns={isbn}&key=SDt8iro5dgDxUjXZf6J7w";
                var responseString = await _httpClient.GetStringAsync(url);
                var response = JObject.Parse(responseString);
                if (response["books"]?[0] != null)
                {
                    return new CatalogItemGoodreadRating
                    {
                        GoodReadBookId = (string)response["books"][0]["id"],
                        Rating = (string)response["books"][0]["average_rating"],
                        RatingCount = ParseRatingCount((string)response["books"][0]["work_ratings_count"])
                    };
                }
            }
            catch (Exception)
            {
                // ignore
            }

            return null;
        }

        private string ParseRatingCount(string count)
        {
            // 1234567 -> 1,234,567
            if (string.IsNullOrEmpty(count))
                return string.Empty;

            return Regex.Replace(count, @"\B(?=(\d{3})+\b)", ",");
        }
    }
}
