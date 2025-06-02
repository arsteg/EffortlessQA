using System.Net.Http.Json;
using EffortlessQA.Data.Dtos;

namespace EffortlessQA.Client.Services
{
    public class TestCaseService
    {
        private readonly HttpClient _httpClient;

        public TestCaseService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("EffortlessQA.Api");
        }

        public async Task<List<TestCaseDto>> GetTestCasesAsync(int? suiteId = null)
        {
            var url = suiteId.HasValue ? $"/test-cases?suiteId={suiteId}" : "/test-cases";
            return await _httpClient.GetFromJsonAsync<List<TestCaseDto>>(url) ?? new();
        }

        public async Task CreateTestCaseAsync(CreateTestCaseDto testCaseDto)
        {
            var response = await _httpClient.PostAsJsonAsync("/test-cases", testCaseDto);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteTestCaseAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"/test-cases/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
