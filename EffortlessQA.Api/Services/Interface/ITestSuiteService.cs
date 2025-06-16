using EffortlessQA.Data.Dtos;

namespace EffortlessQA.Api.Services.Interface
{
    public interface ITestSuiteService
    {
        Task<TestSuiteDto> CreateTestSuiteAsync(
            Guid projectId,
            string tenantId,
            CreateTestSuiteDto dto
        );
        Task<PagedResult<TestSuiteDto>> GetTestSuitesAsync(
            Guid projectId,
            string tenantId,
            int page,
            int limit,
            string? filter
        );
		Task<PagedResult<TestSuiteDto>> GetTestSuitesAsync(
			string tenantId,
			int page,
			int limit,
			string? filter
		);
		Task<TestSuiteDto> GetTestSuiteAsync(Guid testSuiteId, Guid projectId, string tenantId);
        Task<TestSuiteDto> UpdateTestSuiteAsync(
            Guid testSuiteId,
            Guid projectId,
            string tenantId,
            UpdateTestSuiteDto dto
        );
        Task DeleteTestSuiteAsync(Guid testSuiteId, Guid projectId, string tenantId);
		Task LinkTestSuiteToRequirementAsync(
			Guid requirementId,
			Guid projectId,
			string tenantId,
			Guid testSuiteId
		);
		Task UnlinkTestSuiteFromRequirementAsync(
			Guid requirementId,
			Guid projectId,
			string tenantId,
			Guid testSuiteId
		);
	}
}
