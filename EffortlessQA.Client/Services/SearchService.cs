using System.Net.Http.Json;

namespace EffortlessQA.Client.Services
{
    public class SearchService
    {
        private readonly HttpClient _httpClient;

        public SearchService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("EffortlessQA.Api");
        }

        public async Task<List<object>> GlobalSearchAsync(string term)
        {
            return await _httpClient.GetFromJsonAsync<List<object>>($"/search?term={term}")
                ?? new();
        }
    }
}
