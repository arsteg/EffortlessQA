using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EffortlessQA.Data.Entities
{
    [Auditable]
    public class TestSuite : EntityBase
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(100)]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public Guid ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        // [Index]
        public Project Project { get; set; }

        [Required, MaxLength(50)]
        //[Index]
        public string TenantId { get; set; }

        // Navigation property
        public List<TestCase> TestCases { get; set; } = new();

        public Guid? ParentSuiteId { get; set; }

        [ForeignKey("ParentSuiteId")]
        public TestSuite? ParentSuite { get; set; }
    }
}
