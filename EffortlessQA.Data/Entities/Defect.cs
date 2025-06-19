using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace EffortlessQA.Data.Entities
{
    [Auditable]
    public class Defect : EntityBase
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(200)]
        public string Title { get; set; }

        public string? Description { get; set; }

        [Required, MaxLength(20)]
        [RegularExpression("High|Medium|Low")]
        // [Index]
        public SeverityLevel Severity { get; set; }

        [Required, MaxLength(20)]
        [RegularExpression("Open|InProgress|Resolved|Closed")]
        // [Index]
        public DefectStatus Status { get; set; }

        public string Attachments { get; set; }

        [MaxLength(100)]
        public string? ExternalId { get; set; } // For Jira/GitHub

        public Guid? TestRunResultId { get; set; }

        [ForeignKey("TestRunResultId")]
        public TestRunResult? TestRunResult { get; set; }

        public Guid? TestCaseId { get; set; }

        [ForeignKey("TestCaseId")]
        public TestCase? TestCase { get; set; }

        [Required, MaxLength(50)]
        // [Index]
        public string TenantId { get; set; }

        public Guid? AssignedUserId { get; set; }

        [ForeignKey("AssignedUserId")]
        public User? AssignedUser { get; set; }

        [MaxLength(1000)]
        public string? ResolutionNotes { get; set; }
    }
}
