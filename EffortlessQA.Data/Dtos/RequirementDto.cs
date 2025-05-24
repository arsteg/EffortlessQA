namespace EffortlessQA.Data.Dtos
{
    public class RequirementDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string[]? Tags { get; set; }
        public Guid ProjectId { get; set; }
        public string TenantId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<Guid> TestCaseIds { get; set; } = new();
    }
}
