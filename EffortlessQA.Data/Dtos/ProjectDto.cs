namespace EffortlessQA.Data.Dtos
{
    public class ProjectDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string TenantId { get; set; }
        public bool IsEditing { get; set; }
        public Guid? ParentRequirementId { get; set; }
    }

    public class CreateProjectDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
    }

    public class UpdateProjectDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }

    public class AssignUserToProjectDto
    {
        public Guid UserId { get; set; }
        public string RoleType { get; set; } // Assumes RoleType is a string or enum
    }
}
