using System.ComponentModel.DataAnnotations;

namespace EffortlessQA.Data.Dtos
{
    public class ProjectCreateDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required, MaxLength(50)]
        public string TenantId { get; set; }
    }
}
