using EffortlessQA.Data.Dtos;
using EffortlessQA.Data.Entities;

namespace EffortlessQA.Api.Services.Interface
{
    public interface IDefectService
    {
        Task<DefectDto> CreateDefectAsync(string tenantId, CreateDefectDto dto);
        Task<PagedResult<DefectDto>> GetDefectsAsync(
            string tenantId,
            int page,
            int limit,
            string? filter,
            SeverityLevel[]? severities,
            DefectStatus[]? statuses
        );
        Task<DefectDto> GetDefectAsync(Guid defectId, string tenantId);
        Task<DefectDto> UpdateDefectAsync(Guid defectId, string tenantId, UpdateDefectDto dto);
        Task DeleteDefectAsync(Guid defectId, string tenantId);
        Task<IList<DefectHistoryDto>> GetDefectHistoryAsync(Guid defectId, string tenantId);
    }
}
