using System.Net.Http.Json;
using System.Text.Json;
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
            return await _httpClient.GetFromJsonAsync<List<ProjectDto>>(
                    _httpClient.BaseAddress?.ToString() + "projects"
                ) ?? new();
        }

        public async Task CreateProjectAsync(CreateProjectDto projectDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(
                    _httpClient.BaseAddress?.ToString() + "projects",
                    projectDto
                );
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex) { }
        }

        public async Task DeleteProjectAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/projects/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<PagedResult<ProjectDto>> GetPagedProjectsAsync(ProjectQuery query)
        {
            try
            {
                var url =
                    $"{_httpClient.BaseAddress?.ToString()}projects/paged?searchTerm={Uri.EscapeDataString(query.SearchTerm ?? "")}"
                    + $"&status={Uri.EscapeDataString(query.Status ?? "")}"
                    + $"&sortBy={query.SortBy}&sortDirection={query.SortDirection}"
                    + $"&page={query.Page}&pageSize={query.PageSize}";

                Console.WriteLine($"Request URL:  {url}");
                var response = await _httpClient.GetAsync(url);
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response: {response.StatusCode} - {responseContent}");

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(
                        $"API call failed: {response.StatusCode} - {responseContent}"
                    );
                }

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return await response.Content.ReadFromJsonAsync<PagedResult<ProjectDto>>(options)
                    ?? new();
            }
            catch (HttpRequestException ex)
                when (ex.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception(
                    "You do not have permission to access projects. Contact an administrator."
                );
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON Parsing Error: {ex.Message}");
                throw new Exception($"Failed to parse API response: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
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
