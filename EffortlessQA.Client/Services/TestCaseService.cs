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
	public class TestCaseService
	{
		private readonly HttpClient _httpClient;

		#region Constructor
		public TestCaseService( IHttpClientFactory httpClientFactory )
		{
			_httpClient = httpClientFactory.CreateClient("EffortlessQA.Api");
		}
		#endregion

		#region CRUD Operations
		public async Task<List<TestCaseDto>> GetTestCasesAsync(
			int page = 1,
			int pageSize = 1000,
			string searchTerm = null,
			CancellationToken cancellationToken = default
		)
		{
			var query = new TestCaseQuery
			{
				Page = page,
				PageSize = pageSize,
				SearchTerm = searchTerm
			};
			var result = await GetPagedTestCasesAsync(query,cancellationToken);
			return result.Items ?? new List<TestCaseDto>();
		}

		public async Task<PagedResult<TestCaseDto>> GetPagedTestCasesAsync(
			TestCaseQuery query,
			CancellationToken cancellationToken = default
		)
		{
			try
			{
				if (query == null)
					throw new ArgumentNullException(nameof(query));
				var url = $"/api/v1/testsuites/testcases?page={query.Page}&size={query.PageSize}&filter={Uri.EscapeDataString(BuildFilter(query) ?? "")}";
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

				var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResult<TestCaseDto>>>(options,cancellationToken);
				Console.WriteLine($"Data Items: {apiResponse?.Data?.Items?.Count ?? 0}");

				if (apiResponse == null || apiResponse.Error != null)
				{
					throw new Exception(
						$"API returned an error: {apiResponse?.Error?.Code ?? "Unknown"} - {apiResponse?.Error?.Message ?? "No response"}"
					);
				}

				return apiResponse.Data ?? new PagedResult<TestCaseDto>();
			}
			catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
			{
				Console.WriteLine($"Unauthorized Error: {ex.Message}");
				throw new Exception("You do not have permission to access test cases. Contact an administrator.",ex);
			}
			catch (JsonException ex)
			{
				Console.WriteLine($"JSON Parsing Error: {ex.Message}");
				throw new Exception($"Failed to parse API response: {ex.Message}",ex);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Unexpected Error: {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception($"Unexpected error while fetching test cases: {ex.Message}",ex);
			}
		}

		public async Task CreateTestCaseAsync(
			CreateTestCaseDto testCaseDto,
			Guid testSuiteId,
			CancellationToken cancellationToken = default
		)
		{
			try
			{
				if (testCaseDto == null)
					throw new ArgumentNullException(nameof(testCaseDto));
				if (string.IsNullOrWhiteSpace(testCaseDto.Title))
					throw new ArgumentException("Test case title is required.",nameof(testCaseDto.Title));

				var response = await _httpClient.PostAsJsonAsync(
					$"/api/v1/testsuites/{testSuiteId}/testcases",
					testCaseDto,
					cancellationToken
				);
				var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
				Console.WriteLine($"CreateTestCase Response: {response.StatusCode} - {responseContent}");

				if (!response.IsSuccessStatusCode)
				{
					throw new HttpRequestException(
						$"Failed to create test case: {response.StatusCode} - {responseContent}",
						null,
						response.StatusCode
					);
				}
			}
			catch (HttpRequestException ex)
			{
				Console.WriteLine($"HTTP Error: {ex.Message}, Status: {ex.StatusCode}");
				throw new Exception(
					$"Failed to create test case: {ex.Message}{(ex.StatusCode.HasValue ? $" (Status: {ex.StatusCode})" : "")}",
					ex
				);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Unexpected Error: {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception($"Unexpected error while creating test case: {ex.Message}",ex);
			}
		}

		public async Task UpdateTestCaseAsync(
			TestCaseDto testCaseDto,
			Guid testCaseId,
			Guid testSuiteId,
			CancellationToken cancellationToken = default
		)
		{
			try
			{
				if (testCaseDto == null)
					throw new ArgumentNullException(nameof(testCaseDto));
				if (string.IsNullOrWhiteSpace(testCaseDto.Title))
					throw new ArgumentException("Test case title is required.",nameof(testCaseDto.Title));

				var response = await _httpClient.PutAsJsonAsync(
					$"/api/v1/testsuites/{testSuiteId}/testcases/{testCaseId}",
					testCaseDto,
					cancellationToken
				);
				var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
				Console.WriteLine($"UpdateTestCase Response: {response.StatusCode} - {responseContent}");

				if (!response.IsSuccessStatusCode)
				{
					throw new HttpRequestException(
						$"Failed to update test case: {response.StatusCode} - {responseContent}",
						null,
						response.StatusCode
					);
				}
			}
			catch (HttpRequestException ex)
			{
				Console.WriteLine($"HTTP Error: {ex.Message}, Status: {ex.StatusCode}");
				throw new Exception(
					$"Failed to update test case: {ex.Message}{(ex.StatusCode.HasValue ? $" (Status: {ex.StatusCode})" : "")}",
					ex
				);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Unexpected Error: {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception($"Unexpected error while updating test case: {ex.Message}",ex);
			}
		}

		public async Task DeleteTestCaseAsync(
			Guid id,
			Guid testSuiteId,
			CancellationToken cancellationToken = default
		)
		{
			try
			{
				if (id == Guid.Empty)
					throw new ArgumentException("Test case ID cannot be empty.",nameof(id));

				var response = await _httpClient.DeleteAsync(
					$"/api/v1/testsuites/{testSuiteId}/testcases/{id}",
					cancellationToken
				);
				var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
				Console.WriteLine($"DeleteTestCase Response: {response.StatusCode} - {responseContent}");

				if (!response.IsSuccessStatusCode)
				{
					throw new HttpRequestException(
						$"Failed to delete test case: {response.StatusCode} - {responseContent}",
						null,
						response.StatusCode
					);
				}
			}
			catch (HttpRequestException ex)
			{
				Console.WriteLine($"HTTP Error: {ex.Message}, Status: {ex.StatusCode}");
				throw new Exception(
					$"Failed to delete test case: {ex.Message}{(ex.StatusCode.HasValue ? $" (Status: {ex.StatusCode})" : "")}",
					ex
				);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Unexpected Error: {ex.Message}\nStackTrace: {ex.StackTrace}");
				throw new Exception($"Unexpected error while deleting test case: {ex.Message}",ex);
			}
		}
		#endregion

		#region Helpers
		private string BuildFilter( TestCaseQuery query )
		{
			var filters = new List<string>();
			if (!string.IsNullOrEmpty(query.SearchTerm))
				filters.Add($"title:{query.SearchTerm}");
			if (!string.IsNullOrEmpty(query.SortBy))
				filters.Add($"sort:{query.SortBy}:{query.SortDirection}");
			return string.Join(",",filters);
		}
		#endregion
	}
}