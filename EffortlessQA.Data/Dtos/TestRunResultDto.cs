using System.ComponentModel.DataAnnotations;
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
        public object? Attachments { get; set; }
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
}
