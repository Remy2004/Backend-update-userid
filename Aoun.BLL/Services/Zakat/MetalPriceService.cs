using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace Aoun.BLL.Services.Zakat
{

    public class MetalPriceService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;
        private readonly  IMemoryCache _cache;
        private const string CacheKey = "MetalPrices";

        public MetalPriceService(HttpClient http, IConfiguration config, IMemoryCache cache)
        {
            _http = http;
            _config = config;
            _cache = cache;
        }
        public async Task<(decimal gold, decimal silver)> GetPrices()
        {
            if (_cache.TryGetValue(CacheKey, out (decimal gold, decimal silver) cachedPrices))
            {
                return cachedPrices;
            }
            var apiKey = _config["GoldApi:ApiKey"] ?? string.Empty;

            try
            {

                var goldPerGram = await FetchPriceFromApi("XAU", apiKey);

                var silverPerGram = await FetchPriceFromApi("XAG", apiKey);

                var prices = (goldPerGram, silverPerGram);

                _cache.Set(CacheKey, prices, TimeSpan.FromHours(1));

                return prices;
            }
            catch
            {
                return (3000, 35); // fallback
            }
        }
        private async Task<decimal> FetchPriceFromApi(string symbol, string apiKey)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://www.goldapi.io/api/{symbol}/EGP/");
            request.Headers.Add("x-access-token", apiKey);

            var response = await _http.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonDocument.Parse(json);

            decimal pricePerOunce = data.RootElement.GetProperty("price").GetDecimal();
            return pricePerOunce / 31.1035m;
        }
    }
}
