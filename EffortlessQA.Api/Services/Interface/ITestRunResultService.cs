using EffortlessQA.Data.Dtos;
using EffortlessQA.Data.Entities;

namespace EffortlessQA.Api.Services.Interface
{
    public interface ITestRunResultService
    {
        Task<TestRunResultDto> CreateTestRunResultAsync(
            Guid testRunId,
            string tenantId,
            CreateTestRunResultDto dto
        );
        Task<PagedResult<TestRunResultDto>> GetTestRunResultsAsync(
            Guid testRunId,
            string tenantId,
            int page,
            int limit,
            TestExecutionStatus[]? statuses
        );
        Task<TestRunResultDto> GetTestRunResultAsync(
            Guid resultId,
            Guid testRunId,
            string tenantId
        );
        Task<TestRunResultDto> UpdateTestRunResultAsync(
            Guid resultId,
            Guid testRunId,
            string tenantId,
            UpdateTestRunResultDto dto
        );
        Task<IList<TestRunResultDto>> BulkUpdateTestRunResultsAsync(
            Guid testRunId,
            string tenantId,
            BulkUpdateTestRunResultDto dto
        );
    }
}
