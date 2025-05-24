using System.ComponentModel.DataAnnotations;

namespace EffortlessQA.Data.Dtos
{
    public class TenantDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? BillingContactEmail { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class TenantCreateDto
    {
        [Required, MaxLength(50)]
        public string Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(255), EmailAddress]
        public string? BillingContactEmail { get; set; }
    }

    public class TenantUpdateDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(255), EmailAddress]
        public string? BillingContactEmail { get; set; }
    }
}
