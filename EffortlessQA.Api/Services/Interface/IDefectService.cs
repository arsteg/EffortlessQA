using EffortlessQA.Data.Dtos;

namespace EffortlessQA.Api.Services.Interface
{
    public interface IDefectService
    {
        Task<PagedResult<DefectDto>> GetDefectsAsync(
            string tenantId,
            int page,
            int limit,
            string sort,
            string filter
        );
        Task<DefectDto> CreateDefectAsync(DefectCreateDto dto, string tenantId);
    }
}
