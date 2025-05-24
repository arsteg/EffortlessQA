using System.ComponentModel.DataAnnotations;

namespace EffortlessQA.Data.Dtos
{
    public class DefectHistoryDto
    {
        public Guid Id { get; set; }
        public Guid DefectId { get; set; }
        public Guid UserId { get; set; }
        public string Action { get; set; }
        public object? Details { get; set; }
        public string TenantId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class DefectHistoryCreateDto
    {
        [Required]
        public Guid DefectId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required, MaxLength(20)]
        public string Action { get; set; }

        public object? Details { get; set; }

        [Required, MaxLength(50)]
        public string TenantId { get; set; }
    }
}
