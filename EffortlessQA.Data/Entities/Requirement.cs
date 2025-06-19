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
        public string? Description { get; set; }

        public string[]? Tags { get; set; } // Stored as text[] in PostgreSQL

        [Required]
        public Guid ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        public Project Project { get; set; }

        [Required, MaxLength(50)]
        public string TenantId { get; set; }

        // Self-referencing nullable foreign key to parent requirement
        public Guid? ParentRequirementId { get; set; }

        [ForeignKey("ParentRequirementId")]
        public Requirement? ParentRequirement { get; set; }

        // Navigation property
        public List<RequirementTestSuite> RequirementTestSuites { get; set; }
    }
}
