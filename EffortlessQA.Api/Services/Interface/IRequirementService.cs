using EffortlessQA.Data.Dtos;

namespace EffortlessQA.Api.Services.Interface
{
    public interface IRequirementService
    {
        Task<PagedResult<RequirementDto>> GetRequirementsAsync(
            Guid projectId,
            string tenantId,
            int page,
            int limit,
            string sort,
            string filter
        );
        Task<RequirementDto> CreateRequirementAsync(
            Guid projectId,
            RequirementCreateDto dto,
            string tenantId
        );
    }
}
