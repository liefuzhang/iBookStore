using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace iBookStoreMVC.Service
{
    public class CurrencyService : ICurrencyService
    {
        private readonly HttpClient _httpClient;

        public CurrencyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<decimal?> GetCurrencyRate(string currency)
        {
            var url = "https://api.exchangeratesapi.io/latest?base=NZD";
            var responseString = await _httpClient.GetStringAsync(url);
            var response = JObject.Parse(responseString);
            return (decimal?)response?["rates"]?[currency];
        }
    }
}
