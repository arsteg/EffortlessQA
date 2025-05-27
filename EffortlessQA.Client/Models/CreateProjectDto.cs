namespace EffortlessQA.Client.Models
{
    public class CreateProjectDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<int> UserIds { get; set; } = new();
    }
}
