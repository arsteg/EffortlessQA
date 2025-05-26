using EffortlessQA.Data.Dtos;

namespace EffortlessQA.Api.Services.Interface
{
    public interface ITestSuiteService
    {
        Task<PagedResult<TestSuiteDto>> GetTestSuitesAsync(
            Guid projectId,
            string tenantId,
            int page,
            int limit,
            string sort,
            string filter
        );
        Task<TestSuiteDto> CreateTestSuiteAsync(
            Guid projectId,
            TestSuiteCreateDto dto,
            string tenantId
        );
        Task<PagedResult<TestCaseDto>> GetTestCasesAsync(
            Guid testSuiteId,
            string tenantId,
            int page,
            int limit,
            string sort,
            string filter
        );
        Task<TestCaseDto> CreateTestCaseAsync(
            Guid testSuiteId,
            TestCaseCreateDto dto,
            string tenantId
        );
    }
}
