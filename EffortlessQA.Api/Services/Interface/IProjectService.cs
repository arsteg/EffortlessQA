using EffortlessQA.Data.Dtos;

namespace EffortlessQA.Api.Services.Interface
{
    public interface IProjectService
    {
        Task<PagedResult<ProjectDto>> GetProjectsAsync(
            string tenantId,
            int page,
            int limit,
            string sort,
            string filter
        );
        Task<ProjectDto> CreateProjectAsync(ProjectCreateDto dto, string tenantId);
    }
}
