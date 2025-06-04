using EffortlessQA.Data.Dtos;

namespace EffortlessQA.Api.Services.Interface
{
    public interface IRequirementService
    {
        Task<RequirementDto> CreateRequirementAsync(
            Guid projectId,
            string tenantId,
            CreateRequirementDto dto
        );
		Task<PagedResult<RequirementDto>> GetRequirementsAsync(
			string tenantId,
			int page,
			int limit,
			string? filter,
			string[]? tags
		);
		Task<PagedResult<RequirementDto>> GetRequirementsAsync(
            Guid projectId,
            string tenantId,
            int page,
            int limit,
            string? filter,
            string[]? tags
        );
        Task<RequirementDto> GetRequirementAsync(
            Guid requirementId,
            Guid projectId,
            string tenantId
        );
        Task<RequirementDto> UpdateRequirementAsync(
            Guid requirementId,
            Guid projectId,
            string tenantId,
            UpdateRequirementDto dto
        );
        Task DeleteRequirementAsync(Guid requirementId, Guid projectId, string tenantId);
        Task LinkTestCaseToRequirementAsync(
            Guid requirementId,
            Guid projectId,
            string tenantId,
            Guid testCaseId
        );
        Task UnlinkTestCaseFromRequirementAsync(
            Guid requirementId,
            Guid projectId,
            string tenantId,
            Guid testCaseId
        );
    }
}
