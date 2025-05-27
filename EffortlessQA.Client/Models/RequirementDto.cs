namespace EffortlessQA.Client.Models
{
    public class RequirementDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int ProjectId { get; set; }
    }
}
