using EffortlessQA.Data.Dtos;

namespace EffortlessQA.Api.Services.Interface
{
    public interface IReportingService
    {
        Task<DashboardDto> GetDashboardDataAsync(Guid projectId, string tenantId);
        Task<TestRunReportDto> GetTestRunReportAsync(
            Guid projectId,
            string tenantId,
            Guid? testRunId,
            string exportFormat
        );
        Task<CoverageReportDto> GetCoverageReportAsync(Guid projectId, string tenantId);
    }
}
