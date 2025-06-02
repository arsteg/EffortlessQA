using System.Net.Http.Json;
using EffortlessQA.Data.Dtos;

namespace EffortlessQA.Client.Services
{
    public class TestSuiteService
    {
        private readonly HttpClient _httpClient;

        public TestSuiteService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("EffortlessQA.Api");
        }

        public async Task<List<TestSuiteDto>> GetTestSuitesAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<TestSuiteDto>>("/test-suites") ?? new();
        }

        public async Task CreateTestSuiteAsync(CreateTestSuiteDto testSuiteDto)
        {
            var response = await _httpClient.PostAsJsonAsync("/test-suites", testSuiteDto);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteTestSuiteAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"/test-suites/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
