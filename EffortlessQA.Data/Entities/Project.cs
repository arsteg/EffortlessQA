using System.ComponentModel.DataAnnotations;

namespace EffortlessQA.Data.Entities
{
    [Auditable]
    public class Project : EntityBase
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(100)]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required, MaxLength(50)]
        // [Index]
        public string TenantId { get; set; }

        // Navigation properties
        public List<TestSuite> TestSuites { get; set; } = new();
        public List<Requirement> Requirements { get; set; } = new();
        public List<TestRun> TestRuns { get; set; } = new();
        public List<Role> Roles { get; set; } = new();
        public List<AuditLog> AuditLogs { get; set; } = new();
    }
}
