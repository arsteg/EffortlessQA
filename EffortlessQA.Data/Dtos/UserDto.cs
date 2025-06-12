using System.ComponentModel.DataAnnotations;
using EffortlessQA.Data.Entities;

namespace EffortlessQA.Data.Dtos
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string TenantId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class UpdateUserDto
    {
        [MaxLength(100)]
        public string? Name { get; set; }

        [EmailAddress, MaxLength(255)]
        public string? Email { get; set; }
    }

    public class PasswordResetRequestDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }
    }

    public class PasswordResetConfirmDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Token { get; set; }

        [Required, MinLength(6)]
        public string NewPassword { get; set; }
    }

    public class InviteUserDto
    {
        [Required, EmailAddress, MaxLength(255)]
        public string Email { get; set; }

        [Required, MaxLength(100)]
        public string FirstName { get; set; }

        [Required, MaxLength(100)]
        public string LastName { get; set; }

        [Required]
        public Guid ProjectId { get; set; }

        [Required]
        public RoleType RoleType { get; set; }
    }

    public class OAuthLoginDto
    {
        [Required]
        public string Token { get; set; }

        [Required, MaxLength(50)]
        public string TenantId { get; set; }
    }
}
