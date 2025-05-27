namespace EffortlessQA.Data.Dtos
{
    public class RoleDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public string RoleType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
