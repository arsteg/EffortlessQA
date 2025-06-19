using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using EffortlessQA.Data.Entities;

namespace EffortlessQA.Data.Dtos
{
    public class TestCaseCreateDto
    {
        [Required, MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public string Steps { get; set; }

        public string ExpectedResults { get; set; }

        [Required, MaxLength(20)]
        [RegularExpression("High|Medium|Low")]
        public PriorityLevel Priority { get; set; }

        public string[]? Tags { get; set; }

        [Required]
        public Guid TestSuiteId { get; set; }

        [Required, MaxLength(50)]
        public string TenantId { get; set; }
    }
}
