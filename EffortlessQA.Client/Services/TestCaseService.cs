using System.Net.Http.Json;
using EffortlessQA.Data.Dtos;

namespace EffortlessQA.Client.Services
{
    public class TestCaseService
    {
        private readonly HttpClient _httpClient;

        public TestCaseService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<TestCaseDto>> GetTestCasesAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<TestCaseDto>>("/api/testcases");
        }
    }
}
