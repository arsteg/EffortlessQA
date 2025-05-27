using EffortlessQA.Api.Services.Interface;
using EffortlessQA.Data;
using EffortlessQA.Data.Dtos;
using Microsoft.EntityFrameworkCore;

namespace EffortlessQA.Api.Services.Implementation
{
    public class AuditLogService : IAuditLogService
    {
        private readonly EffortlessQAContext _context;
        private readonly IConfiguration _configuration;

        public AuditLogService(EffortlessQAContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<PagedResult<AuditLogDto>> GetAuditLogsAsync(
            Guid projectId,
            string tenantId,
            int page,
            int limit,
            string? actionFilter,
            string? entityTypeFilter
        )
        {
            var query = _context.AuditLogs.Where(al => al.TenantId == tenantId);

            if (!string.IsNullOrEmpty(actionFilter))
            {
                query = query.Where(al => al.Action.Contains(actionFilter));
            }

            if (!string.IsNullOrEmpty(entityTypeFilter))
            {
                query = query.Where(al => al.EntityType.Contains(entityTypeFilter));
            }

            query = query.OrderByDescending(al => al.CreatedAt);

            var totalCount = await query.CountAsync();
            var auditLogs = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(al => new AuditLogDto
                {
                    Id = al.Id,
                    Action = al.Action,
                    EntityType = al.EntityType,
                    EntityId = al.EntityId,
                    UserId = al.UserId,
                    // ProjectId = al.ProjectId,
                    TenantId = al.TenantId,
                    //Details = al.Details?.ToString(),
                    CreatedAt = al.CreatedAt
                })
                .ToListAsync();

            return new PagedResult<AuditLogDto>
            {
                Items = auditLogs,
                TotalCount = totalCount,
                Page = page,
                Limit = limit
            };
        }
    }
}
