using System.ComponentModel.DataAnnotations;

namespace EffortlessQA.Data.Dtos
{
    public class UserCreateDto
    {
        [Required, EmailAddress, MaxLength(255)]
        public string Email { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string? Password { get; set; } // For email/password signup

        [MaxLength(50)]
        public string? OAuthProvider { get; set; }

        [MaxLength(255)]
        public string? OAuthId { get; set; }

        [Required, MaxLength(50)]
        public string TenantId { get; set; }
    }
}
