using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Metrics;

namespace EffortlessQA.Data.Entities
{
    [Auditable]
    public class Tenant : EntityBase
    {
        [Key]
        [MaxLength(50)]
        public string Id { get; set; } // Matches TenantId in other entities

        [Required, MaxLength(100)]
        public string Name { get; set; } // Organization name

        [Required]
        public string ContactPerson { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; }

        public long? Phone { get; set; } // Use long to handle all number lengths

        public string? Description { get; set; }

        [MaxLength(255)]
        public string? BillingContactEmail { get; set; } // For SaaS billing

        // Navigation property
        public List<User> Users { get; set; } = new();

        public Address? Address { get; set; }
        public bool IsEmailConfirmed { get; set; }
    }
}
