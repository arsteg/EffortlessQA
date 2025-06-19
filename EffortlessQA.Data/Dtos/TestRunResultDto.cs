using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using EffortlessQA.Data.Entities;

namespace EffortlessQA.Data.Dtos
{
    public class TestRunResultDto
    {
        public Guid Id { get; set; }
        public Guid TestCaseId { get; set; }
        public Guid TestRunId { get; set; }
        public TestExecutionStatus Status { get; set; }
        public string? Comments { get; set; }
        public string Attachments { get; set; }
        public string TenantId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class TestRunResultUpdateDto
    {
        [Required]
        public TestExecutionStatus Status { get; set; }

        [MaxLength(1000)]
        public string? Comments { get; set; }

        public object? Attachments { get; set; }
    }

    public class TestRunResultBulkUpdateDto
    {
        public List<TestRunResultUpdateItem> Updates { get; set; } = new();

        public class TestRunResultUpdateItem
        {
            [Required]
            public Guid TestRunResultId { get; set; }

            [Required]
            public TestExecutionStatus Status { get; set; }

            [MaxLength(1000)]
            public string? Comments { get; set; }

            public object? Attachments { get; set; }
        }
    }

    public class CreateTestRunResultDto
    {
        public Guid TestCaseId { get; set; }
        public TestExecutionStatus Status { get; set; }
        public string? Comments { get; set; }
        public string? Attachments { get; set; } // JSON string
    }

    public class UpdateTestRunResultDto
    {
        public TestExecutionStatus? Status { get; set; }
        public string? Comments { get; set; }
        public string? Attachments { get; set; } // JSON string
    }

    public class BulkUpdateTestRunResultDto
    {
        public List<BulkUpdateTestRunResultItemDto> ResultUpdates { get; set; } = new();
    }

    public class BulkUpdateTestRunResultItemDto
    {
        public Guid ResultId { get; set; }
        public TestExecutionStatus? Status { get; set; }
        public string? Comments { get; set; }
        public string? Attachments { get; set; } // JSON string
    }
}
