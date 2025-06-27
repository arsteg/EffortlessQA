using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using EffortlessQA.Data.Dtos;
using EffortlessQA.UI.Models;

namespace EffortlessQA.UI.Services
{
    public class ProjectService
    {
        private readonly HttpClient _httpClient;

        #region Constructor

        public ProjectService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("EffortlessQA.Api");
        }

        #endregion

        #region CRUD Operations

        public async Task<List<ProjectDto>> GetProjectsAsync(
            string searchTerm = null,
            CancellationToken cancellationToken = default
        )
        {
            var query = new ProjectQuery
            {
                Page = 1,
                PageSize = 1000,
                SearchTerm = searchTerm
            };
            var result = await GetPagedProjectsAsync(query, cancellationToken);
            return result.Items ?? new List<ProjectDto>();
        }

        public async Task<PagedResult<ProjectDto>> GetPagedProjectsAsync(
            ProjectQuery query,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                if (query == null)
                    throw new ArgumentNullException(nameof(query));
                var url =
                    $"projects?page={query.Page}&size={query.PageSize}&filter={Uri.EscapeDataString(BuildFilter(query) ?? "")}";
                Console.WriteLine($"Request URL: {url}");

                var response = await _httpClient.GetAsync(url, cancellationToken);
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                Console.WriteLine($"Response: {response.StatusCode} - {responseContent}");

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(
                        $"API call failed: {response.StatusCode} - {responseContent}",
                        null,
                        response.StatusCode
                    );
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    AllowTrailingCommas = true
                };

                var apiResponse = await response.Content.ReadFromJsonAsync<
                    ApiResponse<PagedResult<ProjectDto>>
                >(options, cancellationToken);
                //Console.WriteLine($"API Response: Success={apiResponse?.Success}, Error={apiResponse?.Error?.Code} - {apiResponse?.Error?.Message}");
                Console.WriteLine($"Data Items: {apiResponse?.Data?.Items?.Count ?? 0}");

                if (apiResponse == null || apiResponse.Error != null)
                {
                    throw new Exception(
                        $"API returned an error: {apiResponse?.Error?.Code ?? "Unknown"} - {apiResponse?.Error?.Message ?? "No response"}"
                    );
                }

                return apiResponse.Data ?? new PagedResult<ProjectDto>();
            }
            catch (HttpRequestException ex)
                when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                Console.WriteLine($"Unauthorized Error: {ex.Message}");
                throw new Exception(
                    "You do not have permission to access projects. Contact an administrator.",
                    ex
                );
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON Parsing Error: {ex.Message}");
                throw new Exception($"Failed to parse API response: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error: {ex.Message}\nStackTrace: {ex.StackTrace}");
                throw new Exception($"Unexpected error while fetching projects: {ex.Message}", ex);
            }
        }

        public async Task CreateProjectAsync(
            CreateProjectDto projectDto,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                if (projectDto == null)
                    throw new ArgumentNullException(nameof(projectDto));
                if (string.IsNullOrWhiteSpace(projectDto.Name))
                    throw new ArgumentException(
                        "Project name is required.",
                        nameof(projectDto.Name)
                    );

                var response = await _httpClient.PostAsJsonAsync(
                    $"projects",
                    projectDto,
                    cancellationToken
                );
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                Console.WriteLine(
                    $"CreateProject Response: {response.StatusCode} - {responseContent}"
                );

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(
                        $"Failed to create project: {response.StatusCode} - {responseContent}",
                        null,
                        response.StatusCode
                    );
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP Error: {ex.Message}, Status: {ex.StatusCode}");
                throw new Exception(
                    $"Failed to create project: {ex.Message}{(ex.StatusCode.HasValue ? $" (Status: {ex.StatusCode})" : "")}",
                    ex
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error: {ex.Message}\nStackTrace: {ex.StackTrace}");
                throw new Exception($"Unexpected error while creating project: {ex.Message}", ex);
            }
        }

        public async Task UpdateProjectAsync(
            ProjectDto projectDto,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                if (projectDto == null)
                    throw new ArgumentNullException(nameof(projectDto));
                if (string.IsNullOrWhiteSpace(projectDto.Name))
                    throw new ArgumentException(
                        "Project name is required.",
                        nameof(projectDto.Name)
                    );

                var response = await _httpClient.PutAsJsonAsync(
                    $"projects/{projectDto.Id}",
                    projectDto,
                    cancellationToken
                );
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                Console.WriteLine(
                    $"UpdateProject Response: {response.StatusCode} - {responseContent}"
                );

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(
                        $"Failed to update project: {response.StatusCode} - {responseContent}",
                        null,
                        response.StatusCode
                    );
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP Error: {ex.Message}, Status: {ex.StatusCode}");
                throw new Exception(
                    $"Failed to update project: {ex.Message}{(ex.StatusCode.HasValue ? $" (Status: {ex.StatusCode})" : "")}",
                    ex
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error: {ex.Message}\nStackTrace: {ex.StackTrace}");
                throw new Exception($"Unexpected error while updating project: {ex.Message}", ex);
            }
        }

        public async Task DeleteProjectAsync(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                if (id == Guid.Empty)
                    throw new ArgumentException("Project ID cannot be empty.", nameof(id));

                var response = await _httpClient.DeleteAsync($"projects/{id}", cancellationToken);
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                Console.WriteLine(
                    $"DeleteProject Response: {response.StatusCode} - {responseContent}"
                );

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(
                        $"Failed to delete project: {response.StatusCode} - {responseContent}",
                        null,
                        response.StatusCode
                    );
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP Error: {ex.Message}, Status: {ex.StatusCode}");
                throw new Exception(
                    $"Failed to delete project: {ex.Message}{(ex.StatusCode.HasValue ? $" (Status: {ex.StatusCode})" : "")}",
                    ex
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error: {ex.Message}\nStackTrace: {ex.StackTrace}");
                throw new Exception($"Unexpected error while deleting project: {ex.Message}", ex);
            }
        }

        #endregion



        #region Helpers

        private string BuildFilter(ProjectQuery query)
        {
            var filters = new List<string>();
            if (!string.IsNullOrEmpty(query.SearchTerm))
                filters.Add($"name:{query.SearchTerm}");
            if (!string.IsNullOrEmpty(query.SortBy))
                filters.Add($"sort:{query.SortBy}:{query.SortDirection}");
            return string.Join(",", filters);
        }

        #endregion
    }
}
