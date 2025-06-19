using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace EffortlessQA.Data.Entities
{
    [Auditable]
    public class TestCase : EntityBase
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(200)]
        public string Title { get; set; }

        public string? Description { get; set; }

        public string Steps { get; set; } // JSONB for AI-ready metadata

        public string ExpectedResults { get; set; }

        public string? ActualResult { get; set; }

        public string? Comments { get; set; }

        public string? TestData { get; set; }

        public string? Precondition { get; set; }

        public TestExecutionStatus? Status { get; set; }

        public string? Screenshot { get; set; }

        [Required, MaxLength(20)]
        [RegularExpression("High|Medium|Low")]
        // [Index]
        public PriorityLevel Priority { get; set; }

        public string[]? Tags { get; set; }

        [Required]
        public Guid TestSuiteId { get; set; }

        [ForeignKey("TestSuiteId")]
        //[Index]
        public TestSuite TestSuite { get; set; }

        [Required, MaxLength(50)]
        // [Index]
        public string TenantId { get; set; }

        // Navigation properties
        public List<TestRunResult> TestRunResults { get; set; } = new();
        public List<Defect> Defects { get; set; } = new();
        public List<RequirementTestCase> RequirementTestCases { get; set; } = new();
        public Guid? FolderId { get; set; }

        [ForeignKey("FolderId")]
        public TestFolder? Folder { get; set; }
    }
}
