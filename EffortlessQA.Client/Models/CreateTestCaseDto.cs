namespace EffortlessQA.Client.Models
{
    public class CreateTestCaseDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Steps { get; set; } = string.Empty;
        public string ExpectedResult { get; set; } = string.Empty;
        public int TestSuiteId { get; set; }
    }
}
