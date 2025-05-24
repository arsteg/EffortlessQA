using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace EffortlessQA.Data.Entities
{
    public class AuditLog : EntityBase
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(50)]
        public string Action { get; set; } // e.g., "TestCaseCreated"

        [Required, MaxLength(50)]
        public string EntityType { get; set; } // e.g., "TestCase"

        [Required]
        public Guid EntityId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        public Guid ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        // [Index]
        public Project Project { get; set; }

        [Required, MaxLength(50)]
        //[Index]
        public string TenantId { get; set; }

        public JsonDocument? Details { get; set; } // JSONB for additional context
    }
}
