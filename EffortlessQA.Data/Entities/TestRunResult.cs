using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EffortlessQA.Data.Entities
{
    [Auditable]
    public class TestRunResult : EntityBase
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid TestCaseId { get; set; }

        [ForeignKey("TestCaseId")]
        public TestCase TestCase { get; set; }

        [Required]
        public Guid TestRunId { get; set; }

        [ForeignKey("TestRunId")]
        public TestRun TestRun { get; set; }

        [Required]
        public TestExecutionStatus Status { get; set; }

        [MaxLength(1000)]
        public string? Comments { get; set; }

        public string Attachments { get; set; }

        [Required, MaxLength(50)]
        public string TenantId { get; set; }

        // Navigation property
        public List<Defect> Defects { get; set; } = new();
    }
}
