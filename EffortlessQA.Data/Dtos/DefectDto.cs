using System.ComponentModel.DataAnnotations;
using EffortlessQA.Data.Entities;

namespace EffortlessQA.Data.Dtos
{
    public class DefectDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public SeverityLevel Severity { get; set; }
        public DefectStatus Status { get; set; }
        public object? Attachments { get; set; }
        public string? ExternalId { get; set; }
        public Guid? AssignedUserId { get; set; }
        public string? ResolutionNotes { get; set; }
        public Guid? TestRunResultId { get; set; }
        public Guid? TestCaseId { get; set; }
        public string TenantId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class DefectCreateDto
    {
        [Required, MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public SeverityLevel Severity { get; set; }

        [Required]
        public DefectStatus Status { get; set; }

        public object? Attachments { get; set; }

        [MaxLength(100)]
        public string? ExternalId { get; set; }

        public Guid? AssignedUserId { get; set; }

        [MaxLength(1000)]
        public string? ResolutionNotes { get; set; }

        public Guid? TestRunResultId { get; set; }

        public Guid? TestCaseId { get; set; }

        [Required, MaxLength(50)]
        public string TenantId { get; set; }
    }

    public class DefectUpdateDto
    {
        [Required, MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public SeverityLevel Severity { get; set; }

        [Required]
        public DefectStatus Status { get; set; }

        public object? Attachments { get; set; }

        [MaxLength(100)]
        public string? ExternalId { get; set; }

        public Guid? AssignedUserId { get; set; }

        [MaxLength(1000)]
        public string? ResolutionNotes { get; set; }
    }
}
