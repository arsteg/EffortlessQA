using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EffortlessQA.Data.Entities
{
    [Auditable]
    public class Requirement : EntityBase
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public string[]? Tags { get; set; } // Stored as text[] in PostgreSQL

        [Required]
        public Guid ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        // [Index]
        public Project Project { get; set; }

        [Required, MaxLength(50)]
        // [Index]
        public string TenantId { get; set; }

        // Navigation property
        public List<RequirementTestCase> RequirementTestCases { get; set; } = new();
    }
}
