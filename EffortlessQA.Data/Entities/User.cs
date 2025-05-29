using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace EffortlessQA.Data.Entities
{
    [Auditable]
    public class User : EntityBase
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(255)]
        [EmailAddress]
        public string Email { get; set; }

        [Required, MaxLength(100)]
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [MaxLength(255)]
        public string? PasswordHash { get; set; } // Nullable for OAuth users

        [MaxLength(50)]
        public string? OAuthProvider { get; set; } // e.g., "Google", "GitHub"

        [MaxLength(255)]
        public string? OAuthId { get; set; } // OAuth user ID

        [Required, MaxLength(50)]
        // [Index] // For performance
        public string TenantId { get; set; }

        // Navigation properties
        public List<Role> Roles { get; set; } = new();
        public List<TestRun> AssignedTestRuns { get; set; } = new();
        public bool IsEmailConfirmed { get; set; }
    }
}
