using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EffortlessQA.Data.Entities
{
    [Auditable]
    public class RequirementTestSuite : EntityBase
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid RequirementId { get; set; }

        [ForeignKey("RequirementId")]
        public Requirement Requirement { get; set; }

        [Required]
        public Guid TestSuiteId { get; set; }

        [ForeignKey("TestSuiteId")]
        public TestSuite TestSuite { get; set; }
    }
}
