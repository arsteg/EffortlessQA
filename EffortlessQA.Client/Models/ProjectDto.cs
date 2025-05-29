namespace EffortlessQA.Client.Models
{ // Extended ProjectDto for inline editing and additional fields
    public class ProjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int TenantId { get; set; }
        public string Status { get; set; } = "Active";
        public bool IsEditing { get; set; }
    }

    // Query model for server-side pagination, sorting, and filtering
    public class ProjectQuery
    {
        public string SearchTerm { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string SortBy { get; set; } = string.Empty;
        public string SortDirection { get; set; } = "asc";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    // Paged result model
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
    }

    public class CreateProjectDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "Active";
        public List<int> UserIds { get; set; } = new();
    }
}
