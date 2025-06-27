namespace EffortlessQA.UI.Models
{
    public class TestRunQuery
    {
        public string SearchTerm { get; set; } = string.Empty;
        public string SortBy { get; set; } = string.Empty;
        public string SortDirection { get; set; } = "asc";
        public int Page { get; set; } = 0;
        public int PageSize { get; set; } = 10;
    }
}
