using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EffortlessQA.Data.Entities;

namespace EffortlessQA.Data.Dtos
{
    public class DashboardDataDto
    {
        public int PassedTests { get; set; }
        public int FailedTests { get; set; }
        public int PendingTests { get; set; }
        public int HighPriorityDefects { get; set; }
        public int MediumPriorityDefects { get; set; }
        public int LowPriorityDefects { get; set; }
        public double TestCoverage { get; set; } // Percentage (0-100)
        public List<TestRunDto> RecentTestRuns { get; set; } = new();
    }

    public class DefectCountsDto
    {
        public int High { get; set; }
        public int Medium { get; set; }
        public int Low { get; set; }
    }

    public class TestRunReportDto
    {
        public int TotalTests { get; set; }
        public int PassCount { get; set; }
        public int FailCount { get; set; }
        public int BlockedCount { get; set; }
        public int SkippedCount { get; set; }
        public List<TestRunResultSummaryDto> Results { get; set; }
        public byte[]? ExportData { get; set; }
        public string? ExportContentType { get; set; }
        public string? ExportFileName { get; set; }
    }

    public class TestRunResultSummaryDto
    {
        public Guid TestCaseId { get; set; }
        public string TestCaseTitle { get; set; }
        public TestExecutionStatus Status { get; set; }
        public string? Comments { get; set; }
        public Guid TestRunId { get; set; }
        public string TestRunName { get; set; }
    }

    public class CoverageReportDto
    {
        public int TotalRequirements { get; set; }
        public int CoveredRequirements { get; set; }
        public double CoveragePercentage { get; set; }
        public List<RequirementCoverageDto> Requirements { get; set; }
    }

    public class RequirementCoverageDto
    {
        public Guid RequirementId { get; set; }
        public string RequirementTitle { get; set; }
        public int TestCaseCount { get; set; }
        public bool IsCovered { get; set; }
    }
}
