using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EffortlessQA.Data.Entities
{
    [Auditable]
    public class UserProject : EntityBase
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

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

        public string? Preferences { get; set; } // JSONB for user-specific settings
    }
}
