using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EffortlessQA.Data.Entities
{
    [Auditable]
    public class RequirementTestCase : EntityBase
    {
        [Required]
        public Guid RequirementId { get; set; }

        [ForeignKey("RequirementId")]
        public Requirement Requirement { get; set; }

        [Required]
        public Guid TestCaseId { get; set; }

        [ForeignKey("TestCaseId")]
        public TestCase TestCase { get; set; }
        public int? Weight { get; set; } // Optional for future prioritization
    }
}
