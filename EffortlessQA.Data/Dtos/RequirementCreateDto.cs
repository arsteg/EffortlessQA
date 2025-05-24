using System.ComponentModel.DataAnnotations;

namespace EffortlessQA.Data.Dtos
{
    public class RequirementCreateDto
    {
        [Required, MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public string[]? Tags { get; set; }

        [Required]
        public Guid ProjectId { get; set; }

        [Required, MaxLength(50)]
        public string TenantId { get; set; }

        public List<Guid>? TestCaseIds { get; set; }
    }
}
