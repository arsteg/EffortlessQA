﻿using System.Net.Http.Json;
using EffortlessQA.Data.Dtos;

namespace EffortlessQA.Client.Services
{
    public class TestRunService
    {
        private readonly HttpClient _httpClient;

        public TestRunService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("EffortlessQA.Api");
        }

        public async Task<List<TestRunDto>> GetTestRunsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<TestRunDto>>("/test-runs") ?? new();
        }

        public async Task CreateTestRunAsync(CreateTestRunDto testRunDto)
        {
            var response = await _httpClient.PostAsJsonAsync("/test-runs", testRunDto);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteTestRunAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"/test-runs/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
