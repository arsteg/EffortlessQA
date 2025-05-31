using System.Net.Http.Json;
using System.Text.Json;
using EffortlessQA.Client.Models;
using EffortlessQA.Data.Dtos;

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
            try
            {
                // Construct the correct endpoint
                var url = $"{_httpClient.BaseAddress?.ToString()}projects?page=1&size=1000";
                Console.WriteLine($"Calling API: {url}");

                // Send the HTTP GET request
                var response = await _httpClient.GetAsync(url);

                // Log the response status and content for debugging
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response Status: {response.StatusCode}");
                Console.WriteLine($"Response Content: {responseContent}");

                // Check if the response is successful
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(
                        $"API call failed with status {response.StatusCode}: {responseContent}",
                        null,
                        response.StatusCode
                    );
                }

                // Deserialize the JSON response
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    // Allow trailing commas and comments for flexibility
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    AllowTrailingCommas = true
                };

                try
                {
                    var apiResponse = await response.Content.ReadFromJsonAsync<
                        ApiResponse<PagedResult<ProjectDto>>
                    >(options);
                    if (apiResponse == null)
                    {
                        throw new JsonException("Deserialized API response is null.");
                    }

                    if (apiResponse != null)
                    {
                        //throw new ApiException(
                        //    $"API returned an error: {apiResponse.Error.Code} - {apiResponse.Error.Message}"
                        //);
                    }

                    // Extract the project list from the nested structure
                    var projects = apiResponse.Data.Items ?? new List<ProjectDto>();
                    //Console.WriteLine($"Parsed {projects.Count} projects from response.");
                    return projects;
                }
                catch (JsonException ex)
                {
                    // Log detailed JSON parsing error
                    Console.WriteLine($"JSON Parsing Error: {ex.Message}");
                    Console.WriteLine($"Response Content: {responseContent}");
                    throw new JsonException(
                        $"Failed to parse API response JSON: {ex.Message}. Raw response: {responseContent}",
                        ex
                    );
                }
            }
            catch (HttpRequestException ex)
            {
                // Handle HTTP-related errors (e.g., 401, 403, 500)
                Console.WriteLine($"HTTP Error: {ex.Message}, Status: {ex.StatusCode}");
                throw new Exception(
                    $"Failed to fetch projects from API: {ex.Message}{(ex.StatusCode.HasValue ? $" (Status: {ex.StatusCode})" : "")}",
                    ex
                );
            }
            catch (Exception ex)
            {
                // Catch any other unexpected errors
                Console.WriteLine($"Unexpected Error: {ex.Message}\nStackTrace: {ex.StackTrace}");
                throw new Exception($"Unexpected error while fetching projects: {ex.Message}", ex);
            }
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

        public async Task DeleteProjectAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"/projects/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<PagedResult<ProjectDto>> GetPagedProjectsAsync(ProjectQuery query)
        {
            try
            {
                var url =
                    $"{_httpClient.BaseAddress?.ToString()}projects?page={query.Page}&limit={query.PageSize}"
                    + $"&filter={Uri.EscapeDataString(BuildFilter(query) ?? "")}";

                Console.WriteLine($"Request URL: {url}");
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
                var apiResponse = await response.Content.ReadFromJsonAsync<
                    ApiResponse<PagedResult<ProjectDto>>
                >(options);
                return apiResponse?.Data ?? new PagedResult<ProjectDto>();
            }
            catch (HttpRequestException ex)
                when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
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

        // Helper method to build filter from ProjectQuery
        private string BuildFilter(ProjectQuery query)
        {
            var filters = new List<string>();
            if (!string.IsNullOrEmpty(query.SearchTerm))
                filters.Add($"name:{query.SearchTerm}");
            if (!string.IsNullOrEmpty(query.Status))
                filters.Add($"status:{query.Status}");
            if (!string.IsNullOrEmpty(query.SortBy))
                filters.Add($"sort:{query.SortBy}:{query.SortDirection}");
            return string.Join(",", filters);
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
