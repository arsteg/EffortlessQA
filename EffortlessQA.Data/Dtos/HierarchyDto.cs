using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EffortlessQA.Data.Entities;

namespace EffortlessQA.Data.Dtos
{
    namespace EffortlessQA.Data.Dtos
    {
        public class ProjectHierarchyDto
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string? Description { get; set; }
            public string TenantId { get; set; }
            public List<RequirementHierarchyDto> Requirements { get; set; } = new();
            public List<TestSuiteHierarchyDto> TestSuites { get; set; } = new();
            public List<TestRunHierarchyDto> TestRuns { get; set; } = new();
        }

        public class RequirementHierarchyDto
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public string? Description { get; set; }
            public string[]? Tags { get; set; }
            public Guid? ParentRequirementId { get; set; }
            public List<RequirementHierarchyDto> ChildRequirements { get; set; } = new();
            public List<TestSuiteHierarchyDto> TestSuites { get; set; } = new();
        }

        public class TestSuiteHierarchyDto
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string? Description { get; set; }
            public Guid? ParentSuiteId { get; set; }
            public List<TestSuiteHierarchyDto> ChildSuites { get; set; } = new();
            public List<TestCaseHierarchyDto> TestCases { get; set; } = new();
        }

        public class TestCaseHierarchyDto
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public string? Description { get; set; }
            public string? Precondition { get; set; }
            public string? TestData { get; set; }
            public string? ActualResult { get; set; }
            public string? Comments { get; set; }
            public string? Screenshot { get; set; }
            public PriorityLevel Priority { get; set; }
            public TestExecutionStatus? Status { get; set; }
            public string[]? Tags { get; set; }
            public List<TestRunResultHierarchyDto> TestRunResults { get; set; } = new();
            public List<DefectHierarchyDto> Defects { get; set; } = new();
        }

        public class TestRunHierarchyDto
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string? Description { get; set; }
            public Guid? AssignedTesterId { get; set; }
            public List<TestRunResultHierarchyDto> TestRunResults { get; set; } = new();
        }

        public class TestRunResultHierarchyDto
        {
            public Guid Id { get; set; }
            public Guid TestCaseId { get; set; }
            public TestExecutionStatus Status { get; set; }
            public string? Comments { get; set; }
            public List<DefectHierarchyDto> Defects { get; set; } = new();
        }

        public class DefectHierarchyDto
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public string? Description { get; set; }
            public SeverityLevel Severity { get; set; }
            public DefectStatus Status { get; set; }
            public string? ExternalId { get; set; }
            public Guid? AssignedUserId { get; set; }
            public string? ResolutionNotes { get; set; }
        }
    }
}
