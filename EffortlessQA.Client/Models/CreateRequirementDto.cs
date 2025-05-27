namespace EffortlessQA.Client.Models
{
    public class CreateRequirementDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int ProjectId { get; set; }
        public List<int> TestCaseIds { get; set; } = new();
    }
}
