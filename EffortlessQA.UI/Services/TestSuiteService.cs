using System.Net.Http.Json;
using System.Text.Json;
using DocumentFormat.OpenXml.Office2010.Excel;
using EffortlessQA.Data.Dtos;
using EffortlessQA.Data.Entities;
using EffortlessQA.UI.Models;

namespace EffortlessQA.UI.Services
{
    public class TestSuiteService
    {
        private readonly HttpClient _httpClient;

        #region Constructor

        public TestSuiteService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("EffortlessQAApi");
        }

        #endregion

        #region CRUD Operations

        public async Task<List<TestSuiteDto>> GetTestSuitesAsync(
            int page = 1,
            int pageSize = 1000,
            string searchTerm = null,
            CancellationToken cancellationToken = default
        )
        {
            var query = new TestSuiteQuery
            {
                Page = 1,
                PageSize = 1000,
                SearchTerm = searchTerm
            };
            var result = await GetPagedTestSuitesAsync(query, cancellationToken);
            return result.Items ?? new List<TestSuiteDto>();
        }

        public async Task<PagedResult<TestSuiteDto>> GetPagedTestSuitesAsync(
            TestSuiteQuery query,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                if (query == null)
                    throw new ArgumentNullException(nameof(query));
                var url =
                    $"/api/v1/projects/testsuites?page={query.Page}&size={query.PageSize}&filter={Uri.EscapeDataString(BuildFilter(query) ?? "")}";
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
                    ApiResponse<PagedResult<TestSuiteDto>>
                >(options, cancellationToken);
                Console.WriteLine($"Data Items: {apiResponse?.Data?.Items?.Count ?? 0}");

                if (apiResponse == null || apiResponse.Error != null)
                {
                    throw new Exception(
                        $"API returned an error: {apiResponse?.Error?.Code ?? "Unknown"} - {apiResponse?.Error?.Message ?? "No response"}"
                    );
                }

                return apiResponse.Data ?? new PagedResult<TestSuiteDto>();
            }
            catch (HttpRequestException ex)
                when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                Console.WriteLine($"Unauthorized Error: {ex.Message}");
                throw new Exception(
                    "You do not have permission to access test suites. Contact an administrator.",
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
                    $"Unexpected error while fetching test suites: {ex.Message}",
                    ex
                );
            }
        }

        public async Task CreateTestSuiteAsync(
            CreateTestSuiteDto testSuiteDto,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                if (testSuiteDto == null)
                    throw new ArgumentNullException(nameof(testSuiteDto));
                if (string.IsNullOrWhiteSpace(testSuiteDto.Name))
                    throw new ArgumentException(
                        "Test suite name is required.",
                        nameof(testSuiteDto.Name)
                    );

                var response = await _httpClient.PostAsJsonAsync(
                    $"/api/v1/projects/{testSuiteDto.ProjectId}/testsuites",
                    testSuiteDto,
                    cancellationToken
                );
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                Console.WriteLine(
                    $"CreateTestSuite Response: {response.StatusCode} - {responseContent}"
                );

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(
                        $"Failed to create test suite: {response.StatusCode} - {responseContent}",
                        null,
                        response.StatusCode
                    );
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP Error: {ex.Message}, Status: {ex.StatusCode}");
                throw new Exception(
                    $"Failed to create test suite: {ex.Message}{(ex.StatusCode.HasValue ? $" (Status: {ex.StatusCode})" : "")}",
                    ex
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error: {ex.Message}\nStackTrace: {ex.StackTrace}");
                throw new Exception(
                    $"Unexpected error while creating test suite: {ex.Message}",
                    ex
                );
            }
        }

        public async Task<Guid> CreateTestSuiteWithIdAsync(
            CreateTestSuiteDto testSuiteDto,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                if (testSuiteDto == null)
                    throw new ArgumentNullException(nameof(testSuiteDto));
                if (string.IsNullOrWhiteSpace(testSuiteDto.Name))
                    throw new ArgumentException(
                        "Test suite name is required.",
                        nameof(testSuiteDto.Name)
                    );

                var response = await _httpClient.PostAsJsonAsync(
                    $"/api/v1/projects/{testSuiteDto.ProjectId}/testsuites",
                    testSuiteDto,
                    cancellationToken
                );
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                Console.WriteLine(
                    $"CreateTestSuite Response: {response.StatusCode} - {responseContent}"
                );

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(
                        $"Failed to create test suite: {response.StatusCode} - {responseContent}",
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
                    ApiResponse<TestSuiteDto>
                >(options, cancellationToken);
                if (apiResponse == null || apiResponse.Error != null)
                {
                    throw new Exception(
                        $"API returned an error: {apiResponse?.Error?.Code ?? "Unknown"} - {apiResponse?.Error?.Message ?? "No response"}"
                    );
                }

                return apiResponse.Data?.Id
                    ?? throw new Exception("Created test suite ID not found in response.");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP Error: {ex.Message}, Status: {ex.StatusCode}");
                throw new Exception(
                    $"Failed to create test suite: {ex.Message}{(ex.StatusCode.HasValue ? $" (Status: {ex.StatusCode})" : "")}",
                    ex
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error: {ex.Message}\nStackTrace: {ex.StackTrace}");
                throw new Exception(
                    $"Unexpected error while creating test suite: {ex.Message}",
                    ex
                );
            }
        }

        public async Task UpdateTestSuiteAsync(
            TestSuiteDto testSuiteDto,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                if (testSuiteDto == null)
                    throw new ArgumentNullException(nameof(testSuiteDto));
                if (string.IsNullOrWhiteSpace(testSuiteDto.Name))
                    throw new ArgumentException(
                        "Test suite name is required.",
                        nameof(testSuiteDto.Name)
                    );

                var response = await _httpClient.PutAsJsonAsync(
                    $"/api/v1/projects/{testSuiteDto.ProjectId}/testsuites/{testSuiteDto.Id}",
                    testSuiteDto,
                    cancellationToken
                );
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                Console.WriteLine(
                    $"UpdateTestSuite Response: {response.StatusCode} - {responseContent}"
                );

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(
                        $"Failed to update test suite: {response.StatusCode} - {responseContent}",
                        null,
                        response.StatusCode
                    );
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP Error: {ex.Message}, Status: {ex.StatusCode}");
                throw new Exception(
                    $"Failed to update test suite: {ex.Message}{(ex.StatusCode.HasValue ? $" (Status: {ex.StatusCode})" : "")}",
                    ex
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error: {ex.Message}\nStackTrace: {ex.StackTrace}");
                throw new Exception(
                    $"Unexpected error while updating test suite: {ex.Message}",
                    ex
                );
            }
        }

        public async Task DeleteTestSuiteAsync(
            Guid id,
            Guid projectId,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                if (id == Guid.Empty)
                    throw new ArgumentException("Test suite ID cannot be empty.", nameof(id));

                var response = await _httpClient.DeleteAsync(
                    $"/api/v1/projects/{projectId}/testsuites/{id}",
                    cancellationToken
                );
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                Console.WriteLine(
                    $"DeleteTestSuite Response: {response.StatusCode} - {responseContent}"
                );

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(
                        $"Failed to delete test suite: {response.StatusCode} - {responseContent}",
                        null,
                        response.StatusCode
                    );
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP Error: {ex.Message}, Status: {ex.StatusCode}");
                throw new Exception(
                    $"Failed to delete test suite: {ex.Message}{(ex.StatusCode.HasValue ? $" (Status: {ex.StatusCode})" : "")}",
                    ex
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error: {ex.Message}\nStackTrace: {ex.StackTrace}");
                throw new Exception(
                    $"Unexpected error while deleting test suite: {ex.Message}",
                    ex
                );
            }
        }

        #endregion

        #region Requirement Linking Operations

        public async Task LinkTestSuiteToRequirementAsync(
            Guid projectId,
            Guid requirementId,
            LinkTestSuiteDto linkTestSuiteDto,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                if (linkTestSuiteDto == null)
                    throw new ArgumentNullException(nameof(linkTestSuiteDto));
                if (linkTestSuiteDto.TestSuiteId == Guid.Empty)
                    throw new ArgumentException(
                        "Test suite ID cannot be empty.",
                        nameof(linkTestSuiteDto.TestSuiteId)
                    );

                var response = await _httpClient.PostAsJsonAsync(
                    $"/api/v1/projects/{projectId}/requirements/{requirementId}/testsuites",
                    linkTestSuiteDto,
                    cancellationToken
                );
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                Console.WriteLine(
                    $"LinkTestSuiteToRequirement Response: {response.StatusCode} - {responseContent}"
                );

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(
                        $"Failed to link test suite to requirement: {response.StatusCode} - {responseContent}",
                        null,
                        response.StatusCode
                    );
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP Error: {ex.Message}, Status: {ex.StatusCode}");
                throw new Exception(
                    $"Failed to link test suite to requirement: {ex.Message}{(ex.StatusCode.HasValue ? $" (Status: {ex.StatusCode})" : "")}",
                    ex
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error: {ex.Message}\nStackTrace: {ex.StackTrace}");
                throw new Exception(
                    $"Unexpected error while linking test suite to requirement: {ex.Message}",
                    ex
                );
            }
        }

        public async Task UnlinkTestSuiteFromRequirementAsync(
            Guid projectId,
            Guid requirementId,
            Guid testSuiteId,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                if (testSuiteId == Guid.Empty)
                    throw new ArgumentException(
                        "Test suite ID cannot be empty.",
                        nameof(testSuiteId)
                    );

                var response = await _httpClient.DeleteAsync(
                    $"/api/v1/projects/{projectId}/requirements/{requirementId}/testsuites/{testSuiteId}",
                    cancellationToken
                );
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                Console.WriteLine(
                    $"UnlinkTestSuiteFromRequirement Response: {response.StatusCode} - {responseContent}"
                );

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(
                        $"Failed to unlink test suite from requirement: {response.StatusCode} - {responseContent}",
                        null,
                        response.StatusCode
                    );
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP Error: {ex.Message}, Status: {ex.StatusCode}");
                throw new Exception(
                    $"Failed to unlink test suite from requirement: {ex.Message}{(ex.StatusCode.HasValue ? $" (Status: {ex.StatusCode})" : "")}",
                    ex
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error: {ex.Message}\nStackTrace: {ex.StackTrace}");
                throw new Exception(
                    $"Unexpected error while unlinking test suite from requirement: {ex.Message}",
                    ex
                );
            }
        }

        #endregion

        #region Helpers

        private string BuildFilter(TestSuiteQuery query)
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
