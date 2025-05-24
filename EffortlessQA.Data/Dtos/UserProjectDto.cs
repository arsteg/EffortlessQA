using System.ComponentModel.DataAnnotations;

namespace EffortlessQA.Data.Dtos
{
    public class UserProjectDto
    {
        public Guid UserId { get; set; }
        public Guid ProjectId { get; set; }
        public string TenantId { get; set; }
        public object? Preferences { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class UserProjectCreateDto
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid ProjectId { get; set; }

        [Required, MaxLength(50)]
        public string TenantId { get; set; }

        public object? Preferences { get; set; }
    }

    public class UserProjectUpdateDto
    {
        public object? Preferences { get; set; }
    }
}
