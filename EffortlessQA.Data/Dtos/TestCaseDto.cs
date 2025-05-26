using EffortlessQA.Data.Entities;

namespace EffortlessQA.Data.Dtos
{
    public class TestCaseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public object? Steps { get; set; } // JSON-compatible
        public object? ExpectedResults { get; set; }
        public PriorityLevel Priority { get; set; }
        public string[]? Tags { get; set; }
        public Guid TestSuiteId { get; set; }
        public string TenantId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateTestCaseDto
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? Steps { get; set; } // JSON string
        public string? ExpectedResults { get; set; } // JSON string
        public PriorityLevel Priority { get; set; }
        public string[]? Tags { get; set; }
        public Guid? FolderId { get; set; }
    }

    public class UpdateTestCaseDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Steps { get; set; } // JSON string
        public string? ExpectedResults { get; set; } // JSON string
        public PriorityLevel? Priority { get; set; }
        public string[]? Tags { get; set; }
        public Guid? FolderId { get; set; }
    }

    public class CopyTestCaseDto
    {
        public Guid TargetTestSuiteId { get; set; }
        public Guid? TargetFolderId { get; set; }
    }

    public class MoveTestCaseDto
    {
        public Guid TargetTestSuiteId { get; set; }
        public Guid? TargetFolderId { get; set; }
    }
}
