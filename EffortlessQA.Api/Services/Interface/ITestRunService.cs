using EffortlessQA.Data.Dtos;

namespace EffortlessQA.Api.Services.Interface
{
    public interface ITestRunService
    {
        Task<TestRunDto> CreateTestRunAsync(Guid projectId, string tenantId, CreateTestRunDto dto);
        Task<PagedResult<TestRunDto>> GetTestRunsAsync(
            Guid projectId,
            string tenantId,
            int page,
            int limit,
            string? filter,
            string[]? statuses
        ); 
        
        Task<PagedResult<TestRunDto>> GetAllTestRunsAsync(
			string tenantId,
			int page,
			int limit,
			string? filter,
			string[]? statuses
		);

		Task<TestRunDto> GetTestRunAsync(Guid testRunId, Guid projectId, string tenantId);
        Task<TestRunDto> UpdateTestRunAsync(
            Guid testRunId,
            Guid projectId,
            string tenantId,
            UpdateTestRunDto dto
        );
        Task DeleteTestRunAsync(Guid testRunId, Guid projectId, string tenantId);
    }
}
