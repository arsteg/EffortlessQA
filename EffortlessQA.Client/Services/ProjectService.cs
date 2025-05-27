using System.Net.Http.Json;
using EffortlessQA.Client.Models;

namespace EffortlessQA.Client.Services
{
    public class ProjectService
    {
        private readonly HttpClient _httpClient;

        public ProjectService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("EffortlessQA.Api");
        }

        public async Task<List<ProjectDto>> GetProjectsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<ProjectDto>>("/projects") ?? new();
        }

        public async Task CreateProjectAsync(CreateProjectDto projectDto)
        {
            var response = await _httpClient.PostAsJsonAsync("/projects", projectDto);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteProjectAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/projects/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
