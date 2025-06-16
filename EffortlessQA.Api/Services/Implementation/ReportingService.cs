using System.Text;
using EffortlessQA.Api.Services.Interface;
using EffortlessQA.Data;
using EffortlessQA.Data.Dtos;
using EffortlessQA.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EffortlessQA.Api.Services.Implementation
{
    public class ReportingService : IReportingService
    {
        private readonly EffortlessQAContext _context;
        private readonly IConfiguration _configuration;

        public ReportingService(EffortlessQAContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<DashboardDataDto> GetDashboardDataAsync(Guid projectId, string tenantId)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p =>
                p.Id == projectId && p.TenantId == tenantId && !p.IsDeleted
            );

            if (project == null)
                throw new Exception("Project not found.");

            // Active Test Runs (runs with results in the last 30 days)
            var activeTestRunsCount = await _context
                .TestRuns.Where(tr =>
                    tr.ProjectId == projectId
                    && tr.TenantId == tenantId
                    && !tr.IsDeleted
                    && tr.TestRunResults.Any(trr => trr.CreatedAt >= DateTime.UtcNow.AddDays(-30))
                )
                .CountAsync();

            // Pass/Fail Rates (last 30 days)
            var testRunResults = await _context
                .TestRunResults.Where(trr =>
                    trr.TestRun.ProjectId == projectId
                    && trr.TenantId == tenantId
                    && trr.CreatedAt >= DateTime.UtcNow.AddDays(-30)
                )
                .ToListAsync();

            var totalResults = testRunResults.Count;
            var passCount = testRunResults.Count(trr => trr.Status == TestExecutionStatus.Passed);
            var failCount = testRunResults.Count(trr => trr.Status == TestExecutionStatus.Failed);
            var passRate = totalResults > 0 ? (passCount * 100.0 / totalResults) : 0;
            var failRate = totalResults > 0 ? (failCount * 100.0 / totalResults) : 0;

            // Defect Counts by Severity (Open or InProgress)
            var defectCounts = await _context
                .Defects.Where(d =>
                    d.TestRunResult.TestRun.ProjectId == projectId
                    && d.TenantId == tenantId
                    && !d.IsDeleted
                    && (d.Status == DefectStatus.Open || d.Status == DefectStatus.InProgress)
                )
                .GroupBy(d => d.Severity)
                .Select(g => new { Severity = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.Severity, g => g.Count);

            var highDefects = defectCounts.GetValueOrDefault(SeverityLevel.High, 0);
            var mediumDefects = defectCounts.GetValueOrDefault(SeverityLevel.Medium, 0);
            var lowDefects = defectCounts.GetValueOrDefault(SeverityLevel.Low, 0);

            // Test Coverage (Requirements linked to Test Cases)
            var totalRequirements = await _context
                .Requirements.Where(r =>
                    r.ProjectId == projectId && r.TenantId == tenantId && !r.IsDeleted
                )
                .CountAsync();
            var coveredRequirements = await _context
                .RequirementTestCases.Where(rtc =>
                    rtc.Requirement.ProjectId == projectId
                    && rtc.Requirement.TenantId == tenantId
                    && !rtc.Requirement.IsDeleted
                    && !rtc.TestCase.IsDeleted
                )
                .Select(rtc => rtc.RequirementId)
                .Distinct()
                .CountAsync();
            var coveragePercentage =
                totalRequirements > 0 ? (coveredRequirements * 100.0 / totalRequirements) : 0;

            return new DashboardDataDto
            {
                //ActiveTestRuns = activeTestRunsCount,
                //PassRate = passRate,
                //FailRate = failRate,
                //DefectCounts = new DefectCountsDto
                //{
                //    High = highDefects,
                //    Medium = mediumDefects,
                //    Low = lowDefects
                //},
                TestCoverage = coveragePercentage
            };
        }

        public async Task<TestRunReportDto> GetTestRunReportAsync(
            Guid projectId,
            string tenantId,
            Guid? testRunId,
            string exportFormat
        )
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p =>
                p.Id == projectId && p.TenantId == tenantId && !p.IsDeleted
            );

            if (project == null)
                throw new Exception("Project not found.");

            var query = _context.TestRunResults.Where(trr =>
                trr.TestRun.ProjectId == projectId && trr.TenantId == tenantId
            );

            if (testRunId.HasValue)
            {
                var testRun = await _context.TestRuns.FirstOrDefaultAsync(tr =>
                    tr.Id == testRunId
                    && tr.ProjectId == projectId
                    && tr.TenantId == tenantId
                    && !tr.IsDeleted
                );
                if (testRun == null)
                    throw new Exception("Test run not found.");
                query = query.Where(trr => trr.TestRunId == testRunId);
            }

            var results = await query
                .Select(trr => new TestRunResultSummaryDto
                {
                    TestCaseId = trr.TestCaseId,
                    TestCaseTitle = trr.TestCase.Title,
                    Status = trr.Status,
                    Comments = trr.Comments,
                    TestRunId = trr.TestRunId,
                    TestRunName = trr.TestRun.Name
                })
                .ToListAsync();

            var totalTests = results.Count;
            var passCount = results.Count(r => r.Status == TestExecutionStatus.Passed);
            var failCount = results.Count(r => r.Status == TestExecutionStatus.Failed);
            var blockedCount = results.Count(r => r.Status == TestExecutionStatus.Blocked);
            var skippedCount = results.Count(r => r.Status == TestExecutionStatus.Skipped);

            var report = new TestRunReportDto
            {
                TotalTests = totalTests,
                PassCount = passCount,
                FailCount = failCount,
                BlockedCount = blockedCount,
                SkippedCount = skippedCount,
                Results = results
            };

            if (exportFormat == "csv")
            {
                var csvBuilder = new StringBuilder();
                csvBuilder.AppendLine("TestRunName,TestCaseTitle,Status,Comments");
                foreach (var result in results)
                {
                    csvBuilder.AppendLine(
                        $"\"{result.TestRunName}\",\"{result.TestCaseTitle}\",\"{result.Status}\",\"{result.Comments ?? ""}\""
                    );
                }
                report.ExportData = Encoding.UTF8.GetBytes(csvBuilder.ToString());
                report.ExportContentType = "text/csv";
                report.ExportFileName =
                    $"testrun_report_{projectId}_{(testRunId.HasValue ? testRunId : "all")}.csv";
            }
            else if (exportFormat == "pdf")
            {
                // In production, use a PDF library like iTextSharp
                // For simplicity, return a placeholder message
                throw new NotSupportedException(
                    "PDF export not implemented in this example. Use a PDF library like iTextSharp."
                );
            }

            return report;
        }

        public async Task<CoverageReportDto> GetCoverageReportAsync(Guid projectId, string tenantId)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p =>
                p.Id == projectId && p.TenantId == tenantId && !p.IsDeleted
            );

            if (project == null)
                throw new Exception("Project not found.");

            var requirements = await _context
                .Requirements.Where(r =>
                    r.ProjectId == projectId && r.TenantId == tenantId && !r.IsDeleted
                )
                .Select(r => new
                {
                    r.Id,
                    r.Title,
                    TestCaseCount = r.RequirementTestSuites.Count(rtc => !rtc.TestSuite.IsDeleted)
                })
                .ToListAsync();

            var totalRequirements = requirements.Count;
            var coveredRequirements = requirements.Count(r => r.TestCaseCount > 0);
            var coveragePercentage =
                totalRequirements > 0 ? (coveredRequirements * 100.0 / totalRequirements) : 0;

            var report = new CoverageReportDto
            {
                TotalRequirements = totalRequirements,
                CoveredRequirements = coveredRequirements,
                CoveragePercentage = coveragePercentage,
                Requirements = requirements
                    .Select(r => new RequirementCoverageDto
                    {
                        RequirementId = r.Id,
                        RequirementTitle = r.Title,
                        TestCaseCount = r.TestCaseCount,
                        IsCovered = r.TestCaseCount > 0
                    })
                    .ToList()
            };

            return report;
        }
    }
}
