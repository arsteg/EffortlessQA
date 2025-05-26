using EffortlessQA.Data.Dtos;

namespace EffortlessQA.Api.Services.Interface
{
    public interface ITestRunResultService
    {
        Task<TestRunResultDto> CreateTestRunResultAsync(
            Guid testRunId,
            TestRunResultCreateDto dto,
            string tenantId
        );
        Task BulkUpdateTestRunResultsAsync(
            Guid testRunId,
            TestRunResultBulkUpdateDto dto,
            string tenantId
        );
    }
}
