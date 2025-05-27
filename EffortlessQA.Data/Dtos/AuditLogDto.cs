namespace EffortlessQA.Data.Dtos
{
    public class AuditLogDto
    {
        public Guid Id { get; set; }
        public string Action { get; set; }
        public string EntityType { get; set; }
        public Guid EntityId { get; set; }
        public Guid UserId { get; set; }
        public string TenantId { get; set; }
        public object? Details { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
