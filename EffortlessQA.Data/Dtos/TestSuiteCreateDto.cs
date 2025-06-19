using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using EffortlessQA.Data.Entities;

namespace EffortlessQA.Data.Dtos
{
    public class TestRunResultCreateDto
    {
        [Required]
        public Guid TestCaseId { get; set; }

        [Required]
        public Guid TestRunId { get; set; }

        [Required]
        public TestExecutionStatus Status { get; set; }

        [MaxLength(1000)]
        public string? Comments { get; set; }

        public string Attachments { get; set; }

        [Required, MaxLength(50)]
        public string TenantId { get; set; }
    }
}
