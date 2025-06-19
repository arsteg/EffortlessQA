using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EffortlessQA.Data.Entities
{
    [Auditable]
    public class DefectHistory : EntityBase
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid DefectId { get; set; }

        [ForeignKey("DefectId")]
        // [Index]
        public Defect Defect { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required, MaxLength(20)]
        public string Action { get; set; } // e.g., "StatusChanged", "CommentAdded"

        public string? Details { get; set; } // JSONB for status changes, comments

        [Required, MaxLength(50)]
        // [Index]
        public string TenantId { get; set; }
    }
}
