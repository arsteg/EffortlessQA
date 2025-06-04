using EffortlessQA.Data.Entities;
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

    public class RequirementDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string[]? Tags { get; set; }
        public Guid ProjectId { get; set; }
        public string TenantId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<Guid> TestCaseIds { get; set; } = new();
		public bool IsEditing { get; set; }
		public Guid? ParentRequirementId { get; set; }
	}

    public class CreateRequirementDto
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public Guid ProjectId { get; set; }
        public string[]? Tags { get; set; }
        public List<Guid> TestCaseIds { get; set; } = new();
		public Guid? ParentRequirementId { get; set; }
	}

    public class UpdateRequirementDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string[]? Tags { get; set; }
    }

    public class LinkTestCaseDto
    {
        public Guid TestCaseId { get; set; }
    }
}
