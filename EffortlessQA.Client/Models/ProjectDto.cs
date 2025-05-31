using System.Text.Json.Serialization;

namespace EffortlessQA.Client.Models
{ // Extended ProjectDto for inline editing and additional fields
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

    public class PagedResult<T>
    {
        public PagedResultData<T> Data { get; set; } = new();
        public MetaDto Meta { get; set; } = new();
        public object Error { get; set; }
    }

    public class PagedResultData<T>
    {
        [JsonPropertyName("projects")]
        public List<T> Items { get; set; } = new();

        [JsonPropertyName("total_count")]
        public int TotalCount { get; set; }
    }

    public class ProjectDto
    {
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("tenant_id")]
        public int TenantId { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [JsonPropertyName("is_editing")]
        public bool IsEditing { get; set; }
    }

    public class CreateProjectDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "Active";
        public List<int> UserIds { get; set; } = new();
    }
}
