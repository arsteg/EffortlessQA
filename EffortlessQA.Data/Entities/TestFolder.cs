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
    public class TestFolder : EntityBase
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(100)]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public Guid ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        public Project Project { get; set; }

        [Required, MaxLength(50)]
        public string TenantId { get; set; }
        public List<TestCase> TestCases { get; set; } = new();
    }
}
