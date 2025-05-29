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

        public async Task<PagedResult<ProjectDto>> GetPagedProjectsAsync(ProjectQuery query)
        {
            var url =
                $"/projects/paged?searchTerm={Uri.EscapeDataString(query.SearchTerm ?? "")}"
                + $"&status={Uri.EscapeDataString(query.Status ?? "")}"
                + $"&sortBy={query.SortBy}&sortDirection={query.SortDirection}"
                + $"&page={query.Page}&pageSize={query.PageSize}";
            return await _httpClient.GetFromJsonAsync<PagedResult<ProjectDto>>(url) ?? new();
        }

        public async Task UpdateProjectAsync(ProjectDto projectDto)
        {
            var response = await _httpClient.PutAsJsonAsync(
                $"/projects/{projectDto.Id}",
                projectDto
            );
            response.EnsureSuccessStatusCode();
        }
    }
}
