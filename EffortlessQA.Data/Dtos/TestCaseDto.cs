using EffortlessQA.Data.Entities;

namespace EffortlessQA.Data.Dtos
{
    public class TestCaseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public object? Steps { get; set; } // JSON-compatible
        public object? ExpectedResults { get; set; }
        public PriorityLevel Priority { get; set; }
        public string[]? Tags { get; set; }
        public Guid TestSuiteId { get; set; }
        public string TenantId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
