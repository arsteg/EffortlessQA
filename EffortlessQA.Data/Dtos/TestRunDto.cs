namespace EffortlessQA.Data.Dtos
{
    public class TestRunDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public Guid? AssignedTesterId { get; set; }
        public Guid ProjectId { get; set; }
        public string TenantId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
