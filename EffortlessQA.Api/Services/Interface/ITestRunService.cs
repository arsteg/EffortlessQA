using EffortlessQA.Data.Dtos;

namespace EffortlessQA.Api.Services.Interface
{
    public interface ITestRunService
    {
        Task<PagedResult<TestRunDto>> GetTestRunsAsync(
            Guid projectId,
            string tenantId,
            int page,
            int limit,
            string sort,
            string filter
        );
        Task<TestRunDto> CreateTestRunAsync(Guid projectId, TestRunCreateDto dto, string tenantId);
    }
}
