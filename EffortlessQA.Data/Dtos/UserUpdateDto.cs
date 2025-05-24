using System.ComponentModel.DataAnnotations;

namespace EffortlessQA.Data.Dtos
{
    public class UserUpdateDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string? Password { get; set; } // For password reset
    }

    public class RoleUpdateDto
    {
        [Required, MaxLength(20)]
        [RegularExpression("Admin|Tester")]
        public string RoleType { get; set; }
    }

    public class ProjectUpdateDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }
    }

    public class RequirementUpdateDto
    {
        [Required, MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public string[]? Tags { get; set; }

        public List<Guid>? TestCaseIds { get; set; }
    }

    public class RequirementTestCaseCreateDto // No update needed for join table
    {
        [Required]
        public Guid RequirementId { get; set; }

        [Required]
        public Guid TestCaseId { get; set; }
    }

    public class TestRunUpdateDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public Guid? AssignedTesterId { get; set; }
    }

    // No AuditLogUpdateDto as audit logs are system-generated
}
