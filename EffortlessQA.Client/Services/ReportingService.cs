using System.Net.Http.Json;
using EffortlessQA.Client.Models;
using EffortlessQA.Data.Dtos;

namespace EffortlessQA.Client.Services
{
    public class ReportingService
    {
        private readonly HttpClient _httpClient;

        public ReportingService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("EffortlessQA.Api");
        }

        public async Task<DashboardDataDto> GetDashboardDataAsync()
        {
            return await _httpClient.GetFromJsonAsync<DashboardDataDto>("/reports/dashboard")
                ?? new();
        }

        public async Task<List<ReportDto>> GetReportsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<ReportDto>>("/reports") ?? new();
        }

        public async Task GenerateTestRunReportAsync()
        {
            var response = await _httpClient.GetAsync("/reports/test-run");
            response.EnsureSuccessStatusCode();
        }

        public async Task GenerateCoverageReportAsync()
        {
            var response = await _httpClient.GetAsync("/reports/coverage");
            response.EnsureSuccessStatusCode();
        }

        public async Task<string> GetReportUrlAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/reports/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
