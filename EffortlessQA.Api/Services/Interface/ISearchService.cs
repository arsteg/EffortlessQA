using EffortlessQA.Data.Dtos;
using EffortlessQA.Data.Entities;

namespace EffortlessQA.Api.Services.Interface
{
    public interface ISearchService
    {
        Task<PagedResult<SearchResultDto>> GlobalSearchAsync(
            string tenantId,
            string query,
            string[]? tags,
            int page,
            int limit
        );
        Task<PagedResult<RequirementDto>> FilterRequirementsAsync(
            Guid projectId,
            string tenantId,
            string[]? tags,
            int page,
            int limit
        );
        Task<PagedResult<TestCaseDto>> FilterTestCasesAsync(
            Guid projectId,
            string tenantId,
            string[]? tags,
            PriorityLevel[]? priorities,
            TestExecutionStatus[]? statuses,
            int page,
            int limit
        );
        Task<PagedResult<TestRunDto>> FilterTestRunsAsync(
            Guid projectId,
            string tenantId,
            TestExecutionStatus[]? statuses,
            Guid[]? assignedTesterIds,
            int page,
            int limit
        );
        Task<PagedResult<DefectDto>> FilterDefectsAsync(
            Guid projectId,
            string tenantId,
            SeverityLevel[]? severities,
            DefectStatus[]? statuses,
            int page,
            int limit
        );
    }
}
