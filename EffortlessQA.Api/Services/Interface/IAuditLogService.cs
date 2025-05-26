using EffortlessQA.Data.Dtos;

namespace EffortlessQA.Api.Services.Interface
{
    public interface IAuditLogService
    {
        Task<PagedResult<AuditLogDto>> GetAuditLogsAsync(
            Guid projectId,
            string tenantId,
            int page,
            int limit,
            string? actionFilter,
            string? entityTypeFilter
        );
    }
}
