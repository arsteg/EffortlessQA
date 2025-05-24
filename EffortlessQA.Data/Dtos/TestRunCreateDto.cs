using System.ComponentModel.DataAnnotations;

namespace EffortlessQA.Data.Dtos
{
    public class TestRunCreateDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public Guid? AssignedTesterId { get; set; }

        [Required]
        public Guid ProjectId { get; set; }

        [Required, MaxLength(50)]
        public string TenantId { get; set; }

        public List<Guid>? TestCaseIds { get; set; } // For selecting test cases
    }
}
