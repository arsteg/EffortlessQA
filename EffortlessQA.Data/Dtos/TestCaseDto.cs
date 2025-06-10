using EffortlessQA.Data.Entities;

namespace EffortlessQA.Data.Dtos
{
    public class TestCaseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? Steps { get; set; } // JSON-compatible
        public string? ExpectedResults { get; set; }
        public PriorityLevel Priority { get; set; }
        public string[]? Tags { get; set; }
        public Guid TestSuiteId { get; set; }
        public string TenantId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
		public bool IsEditing { get; set; }
		public string? ActualResult { get; set; }
		public string? Comments { get; set; }
		public string? TestData { get; set; }
		public string? Precondition { get; set; }
		public TestExecutionStatus? Status { get; set; }
		public string? Screenshot { get; set; }
	}

    public class CreateTestCaseDto
    {
        public string Title { get; set; }
        public string? Description { get; set; }
		public string TenantId { get; set; }
		public string? Steps { get; set; } // JSON string
        public string? ExpectedResults { get; set; } // JSON string
        public PriorityLevel Priority { get; set; }
        public string[]? Tags { get; set; }
        public Guid? FolderId { get; set; }
		public string? ActualResult { get; set; }
		public string? Comments { get; set; }
		public string? TestData { get; set; }
		public string? Precondition { get; set; }
		public TestExecutionStatus? Status { get; set; }
		public string? Screenshot { get; set; }

		public Guid TestSuiteId { get; set; } = Guid.Parse("c1d859b9-6a8e-43bf-bddc-7a847db4849a");
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
		public string? Comments { get; set; }
		public TestExecutionStatus? Status { get; set; }
		public string? ActualResult { get; set; }
		public string? TestData { get; set; }
		public string? Screenshot { get; set; }
		public string? Precondition { get; set; }
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
