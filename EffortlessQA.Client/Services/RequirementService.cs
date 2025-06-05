using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using EffortlessQA.Client.Components;
using EffortlessQA.Client.Models;
using EffortlessQA.Data.Dtos;
using EffortlessQA.Data.Entities;

namespace EffortlessQA.Client.Services
{
    public class RequirementService
    {
        private readonly HttpClient _httpClient;

        #region Constructor

        public RequirementService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("EffortlessQA.Api");
        }

        #endregion

        #region CRUD Operations

        public async Task<List<RequirementDto>> GetRequirementsAsync(
            int page = 1,
            int pageSize = 1000,
            string searchTerm = null,
            CancellationToken cancellationToken = default
        )
        {
            var query = new RequirementQuery
            {
                Page = 1,
                PageSize = 1000,
                SearchTerm = searchTerm
            };
            var result = await GetPagedRequirementsAsync(query, cancellationToken);
            return result.Items ?? new List<RequirementDto>();
        }

        public async Task<PagedResult<RequirementDto>> GetPagedRequirementsAsync(
            RequirementQuery query,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                if (query == null)
                    throw new ArgumentNullException(nameof(query));
                var url =
                    $"/api/v1/projects/requirements?page={query.Page}&size={query.PageSize}&filter={Uri.EscapeDataString(BuildFilter(query) ?? "")}";
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
                    ApiResponse<PagedResult<RequirementDto>>
                >(options, cancellationToken);
                Console.WriteLine($"Data Items: {apiResponse?.Data?.Items?.Count ?? 0}");

                if (apiResponse == null || apiResponse.Error != null)
                {
                    throw new Exception(
                        $"API returned an error: {apiResponse?.Error?.Code ?? "Unknown"} - {apiResponse?.Error?.Message ?? "No response"}"
                    );
                }

                return apiResponse.Data ?? new PagedResult<RequirementDto>();
            }
            catch (HttpRequestException ex)
                when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                Console.WriteLine($"Unauthorized Error: {ex.Message}");
                throw new Exception(
                    "You do not have permission to access requirements. Contact an administrator.",
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
                throw new Exception(
                    $"Unexpected error while fetching requirements: {ex.Message}",
                    ex
                );
            }
        }

        public async Task CreateRequirementAsync(
            CreateRequirementDto requirementDto,
            Guid projectId,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                if (requirementDto == null)
                    throw new ArgumentNullException(nameof(requirementDto));
                if (string.IsNullOrWhiteSpace(requirementDto.Title))
                    throw new ArgumentException(
                        "Requirement title is required.",
                        nameof(requirementDto.Title)
                    );

                var response = await _httpClient.PostAsJsonAsync(
                    $"/api/v1/projects/{projectId}/requirements",
                    requirementDto,
                    cancellationToken
                );
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                Console.WriteLine(
                    $"CreateRequirement Response: {response.StatusCode} - {responseContent}"
                );

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(
                        $"Failed to create requirement: {response.StatusCode} - {responseContent}",
                        null,
                        response.StatusCode
                    );
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP Error: {ex.Message}, Status: {ex.StatusCode}");
                throw new Exception(
                    $"Failed to create requirement: {ex.Message}{(ex.StatusCode.HasValue ? $" (Status: {ex.StatusCode})" : "")}",
                    ex
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error: {ex.Message}\nStackTrace: {ex.StackTrace}");
                throw new Exception(
                    $"Unexpected error while creating requirement: {ex.Message}",
                    ex
                );
            }
        }

        public async Task UpdateRequirementAsync(
            RequirementDto requirementDto,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                if (requirementDto == null)
                    throw new ArgumentNullException(nameof(requirementDto));
                if (string.IsNullOrWhiteSpace(requirementDto.Title))
                    throw new ArgumentException(
                        "Requirement title is required.",
                        nameof(requirementDto.Title)
                    );

                var response = await _httpClient.PutAsJsonAsync(
                    $"/api/v1/projects/{requirementDto.ProjectId}/requirements/{requirementDto.Id}",
                    requirementDto,
                    cancellationToken
                );
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                Console.WriteLine(
                    $"UpdateRequirement Response: {response.StatusCode} - {responseContent}"
                );

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(
                        $"Failed to update requirement: {response.StatusCode} - {responseContent}",
                        null,
                        response.StatusCode
                    );
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP Error: {ex.Message}, Status: {ex.StatusCode}");
                throw new Exception(
                    $"Failed to update requirement: {ex.Message}{(ex.StatusCode.HasValue ? $" (Status: {ex.StatusCode})" : "")}",
                    ex
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error: {ex.Message}\nStackTrace: {ex.StackTrace}");
                throw new Exception(
                    $"Unexpected error while updating requirement: {ex.Message}",
                    ex
                );
            }
        }

        public async Task DeleteRequirementAsync(
            Guid id,
            Guid ProjectId,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                if (id == Guid.Empty)
                    throw new ArgumentException("Requirement ID cannot be empty.", nameof(id));

                var response = await _httpClient.DeleteAsync(
                    $"/api/v1/projects/{ProjectId}/requirements/{id}",
                    cancellationToken
                );
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                Console.WriteLine(
                    $"DeleteRequirement Response: {response.StatusCode} - {responseContent}"
                );

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(
                        $"Failed to delete requirement: {response.StatusCode} - {responseContent}",
                        null,
                        response.StatusCode
                    );
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP Error: {ex.Message}, Status: {ex.StatusCode}");
                throw new Exception(
                    $"Failed to delete requirement: {ex.Message}{(ex.StatusCode.HasValue ? $" (Status: {ex.StatusCode})" : "")}",
                    ex
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error: {ex.Message}\nStackTrace: {ex.StackTrace}");
                throw new Exception(
                    $"Unexpected error while deleting requirement: {ex.Message}",
                    ex
                );
            }
        }

        #endregion

        #region Helpers

        private string BuildFilter(RequirementQuery query)
        {
            var filters = new List<string>();
            if (!string.IsNullOrEmpty(query.SearchTerm))
                filters.Add($"title:{query.SearchTerm}");
            if (!string.IsNullOrEmpty(query.SortBy))
                filters.Add($"sort:{query.SortBy}:{query.SortDirection}");
            return string.Join(",", filters);
        }

        #endregion
    }
}
