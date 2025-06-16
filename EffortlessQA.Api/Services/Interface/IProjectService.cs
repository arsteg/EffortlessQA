using EffortlessQA.Data.Dtos;
using EffortlessQA.Data.Dtos.EffortlessQA.Data.Dtos;

namespace EffortlessQA.Api.Services.Interface
{
    public interface IProjectService
    {
        Task<ProjectDto> CreateProjectAsync(CreateProjectDto dto, string tenantId);
        Task<PagedResult<ProjectDto>> GetProjectsAsync(
            string tenantId,
            int page,
            int limit,
            string? filter
        );
        Task<ProjectDto> GetProjectAsync(Guid projectId, string tenantId);
        Task<ProjectDto> UpdateProjectAsync(Guid projectId, string tenantId, UpdateProjectDto dto);
        Task DeleteProjectAsync(Guid projectId, string tenantId);
        Task AssignUserToProjectAsync(Guid projectId, string tenantId, AssignUserToProjectDto dto);
        Task RemoveUserFromProjectAsync(Guid projectId, Guid userId, string tenantId);
        Task<ProjectHierarchyDto> GetProjectHierarchyAsync(Guid projectId);
    }
}
