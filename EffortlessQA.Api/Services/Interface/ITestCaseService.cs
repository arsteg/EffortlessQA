using EffortlessQA.Data.Dtos;
using EffortlessQA.Data.Entities;

namespace EffortlessQA.Api.Services.Interface
{
    public interface ITestCaseService
    {
        Task<TestCaseDto> CreateTestCaseAsync(
            Guid testSuiteId,
            string tenantId,
            CreateTestCaseDto dto
        );
        Task<PagedResult<TestCaseDto>> GetTestCasesAsync(
            Guid testSuiteId,
            string tenantId,
            int page,
            int limit,
            string? filter,
            string[]? tags,
            PriorityLevel[]? priorities
        );
		Task<PagedResult<TestCaseDto>> GetTestCasesAsync(
			string tenantId,
			int page,
			int limit,
			string? filter,
			string[]? tags,
			PriorityLevel[]? priorities
		);
		Task<TestCaseDto> GetTestCaseAsync(Guid testCaseId, Guid testSuiteId, string tenantId);
        Task<TestCaseDto> UpdateTestCaseAsync(
            Guid testCaseId,
            Guid testSuiteId,
            string tenantId,
            UpdateTestCaseDto dto
        );
        Task DeleteTestCaseAsync(Guid testCaseId, Guid testSuiteId, string tenantId);
        Task<TestCaseDto> CopyTestCaseAsync(Guid testCaseId, string tenantId, CopyTestCaseDto dto);
        Task<IList<TestCaseDto>> ImportTestCasesAsync(
            Guid testSuiteId,
            string tenantId,
            IFormFile file
        );
        Task<byte[]> ExportTestCasesAsync(Guid testSuiteId, string tenantId);
        Task<TestCaseDto> MoveTestCaseAsync(Guid testCaseId, string tenantId, MoveTestCaseDto dto);
    }
}
