using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using EffortlessQA.Client.Models;
using EffortlessQA.Data.Dtos;
using EffortlessQA.Data.Entities;

namespace EffortlessQA.Client.Services
{
	public class TestRunService
	{
		private readonly HttpClient _httpClient;

		public TestRunService( IHttpClientFactory httpClientFactory )
		{
			_httpClient = httpClientFactory.CreateClient("EffortlessQA.Api");
		}

		public async Task<List<TestRunDto>> GetTestRunsAsync(
			int page = 1,
			int pageSize = 1000,
			string searchTerm = null,
			CancellationToken cancellationToken = default
		)
		{
			var query = new TestRunQuery
			{
				Page = page,
				PageSize = pageSize,
				SearchTerm = searchTerm
			};
			var result = await GetPagedTestRunsAsync(query,cancellationToken);
			return result.Items ?? new List<TestRunDto>();
		}

		public async Task<PagedResult<TestRunDto>> GetPagedTestRunsAsync(
			TestRunQuery query,
			CancellationToken cancellationToken = default
		)
		{
			try
			{
				if (query == null)
					throw new ArgumentNullException(nameof(query));
				var url = $"/api/v1/projects/testruns?page={query.Page}&size={query.PageSize}&filter={Uri.EscapeDataString(BuildFilter(query) ?? "")}";
				Console.WriteLine($"Request URL: {url}");

				var response = await _httpClient.GetAsync(url,cancellationToken);
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
					ApiResponse<PagedResult<TestRunDto>>
				>(options,cancellationToken);
				Console.WriteLine($"Data Items: {apiResponse?.Data?.Items?.Count ?? 0}");

				if (apiResponse == null || apiResponse.Error != null)
				{
					throw new Exception(
						$"API returned an error: {apiResponse?.Error?.Code ?? "Unknown"} - {apiResponse?.Error?.Message ?? "No response"}"
					);
				}

				return apiResponse.Data ?? new PagedResult<TestRunDto>();
			}
			catch (HttpRequestException ex)
				when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
			{
				Console.WriteLine($"Unauthorized Error: {ex.Message}");
				throw new Exception(
					"You do not have permission to access test runs. Contact an administrator.",
					ex
				);
			}
			catch (JsonException ex)
			{
				Console.WriteLine($"JSON Parsing Error: {ex.Message}");
				throw new Exception($"Failed to parse API response: {ex.Message}",ex);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Unexpected Error: {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception(
					$"Unexpected error while fetching test runs: {ex.Message}",
					ex
				);
			}
		}

		public async Task CreateTestRunAsync(
			CreateTestRunDto testRunDto,
			Guid projectId,
			CancellationToken cancellationToken = default
		)
		{
			try
			{
				if (testRunDto == null)
					throw new ArgumentNullException(nameof(testRunDto));
				if (string.IsNullOrWhiteSpace(testRunDto.Name))
					throw new ArgumentException(
						"Test run name is required.",
						nameof(testRunDto.Name)
					);

				var response = await _httpClient.PostAsJsonAsync(
					$"/api/v1/projects/{projectId}/testruns",
					testRunDto,
					cancellationToken
				);
				var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
				Console.WriteLine(
					$"CreateTestRun Response: {response.StatusCode} - {responseContent}"
				);

				if (!response.IsSuccessStatusCode)
				{
					throw new HttpRequestException(
						$"Failed to create test run: {response.StatusCode} - {responseContent}",
						null,
						response.StatusCode
					);
				}
			}
			catch (HttpRequestException ex)
			{
				Console.WriteLine($"HTTP Error: {ex.Message}, Status: {ex.StatusCode}");
				throw new Exception(
					$"Failed to create test run: {ex.Message}{(ex.StatusCode.HasValue ? $" (Status: {ex.StatusCode})" : "")}",
					ex
				);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Unexpected Error: {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception(
					$"Unexpected error while creating test run: {ex.Message}",
					ex
				);
			}
		}

		public async Task UpdateTestRunAsync(
			TestRunDto testRunDto,
			CancellationToken cancellationToken = default
		)
		{
			try
			{
				if (testRunDto == null)
					throw new ArgumentNullException(nameof(testRunDto));
				if (string.IsNullOrWhiteSpace(testRunDto.Name))
					throw new ArgumentException(
						"Test run name is required.",
						nameof(testRunDto.Name)
					);

				var response = await _httpClient.PutAsJsonAsync(
					$"/api/v1/projects/{testRunDto.ProjectId}/testruns/{testRunDto.Id}",
					testRunDto,
					cancellationToken
				);
				var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
				Console.WriteLine(
					$"UpdateTestRun Response: {response.StatusCode} - {responseContent}"
				);

				if (!response.IsSuccessStatusCode)
				{
					throw new HttpRequestException(
						$"Failed to update test run: {response.StatusCode} - {responseContent}",
						null,
						response.StatusCode
					);
				}
			}
			catch (HttpRequestException ex)
			{
				Console.WriteLine($"HTTP Error: {ex.Message}, Status: {ex.StatusCode}");
				throw new Exception(
					$"Failed to update test run: {ex.Message}{(ex.StatusCode.HasValue ? $" (Status: {ex.StatusCode})" : "")}",
					ex
				);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Unexpected Error: {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception(
					$"Unexpected error while updating test run: {ex.Message}",
					ex
				);
			}
		}

		public async Task DeleteTestRunAsync(
			Guid id,
			Guid projectId,
			CancellationToken cancellationToken = default
		)
		{
			try
			{
				if (id == Guid.Empty)
					throw new ArgumentException("Test run ID cannot be empty.",nameof(id));

				var response = await _httpClient.DeleteAsync(
					$"/api/v1/projects/{projectId}/testruns/{id}",
					cancellationToken
				);
				var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
				Console.WriteLine(
					$"DeleteTestRun Response: {response.StatusCode} - {responseContent}"
				);

				if (!response.IsSuccessStatusCode)
				{
					throw new HttpRequestException(
						$"Failed to delete test run: {response.StatusCode} - {responseContent}",
						null,
						response.StatusCode
					);
				}
			}
			catch (HttpRequestException ex)
			{
				Console.WriteLine($"HTTP Error: {ex.Message}, Status: {ex.StatusCode}");
				throw new Exception(
					$"Failed to delete test run: {ex.Message}{(ex.StatusCode.HasValue ? $" (Status: {ex.StatusCode})" : "")}",
					ex
				);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Unexpected Error: {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception(
					$"Unexpected error while deleting test run: {ex.Message}",
					ex
				);
			}
		}

		public async Task<List<UserDto>> GetUsersAsync( CancellationToken cancellationToken = default )
		{
			try
			{
				var response = await _httpClient.GetAsync("/api/v1/users",cancellationToken);
				var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
				Console.WriteLine($"GetUsers Response: {response.StatusCode} - {responseContent}");

				if (!response.IsSuccessStatusCode)
				{
					throw new HttpRequestException(
						$"Failed to fetch users: {response.StatusCode} - {responseContent}",
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

				var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<UserDto>>>(options,cancellationToken);
				return apiResponse?.Data ?? new List<UserDto>();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error fetching users: {ex.Message}");
				throw new Exception($"Failed to fetch users: {ex.Message}",ex);
			}
		}

		private string BuildFilter( TestRunQuery query )
		{
			var filters = new List<string>();
			if (!string.IsNullOrEmpty(query.SearchTerm))
				filters.Add($"name:{query.SearchTerm}");
			if (!string.IsNullOrEmpty(query.SortBy))
				filters.Add($"sort:{query.SortBy}:{query.SortDirection}");
			return string.Join(",",filters);
		}
	}
}