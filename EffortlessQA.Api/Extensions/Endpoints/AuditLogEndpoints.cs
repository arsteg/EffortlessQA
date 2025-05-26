using EffortlessQA.Api.Services.Interface;
using EffortlessQA.Data.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace EffortlessQA.Api.Extensions
{
    public static partial class ApiExtensions
    {
        public static void MapAuditLogEndpoints(this WebApplication app)
        {
            // GET /api/v1/projects/{projectId}/auditlogs
            app.MapGet(
                    "/api/v1/projects/{projectId}/auditlogs",
                    async (
                        Guid projectId,
                        IAuditLogService auditLogService,
                        HttpContext context,
                        [FromQuery] int page = 1,
                        [FromQuery] int limit = 50,
                        [FromQuery] string? action = null,
                        [FromQuery] string? entity = null
                    ) =>
                    {
                        try
                        {
                            var tenantId = context.User.FindFirst("TenantId")?.Value;
                            if (string.IsNullOrEmpty(tenantId))
                            {
                                return Results.Unauthorized();
                            }
                            var auditLogs = await auditLogService.GetAuditLogsAsync(
                                projectId,
                                tenantId,
                                page,
                                limit,
                                action,
                                entity
                            );
                            return Results.Ok(
                                new ApiResponse<PagedResult<AuditLogDto>>
                                {
                                    Data = auditLogs,
                                    Meta = new
                                    {
                                        Page = page,
                                        Limit = limit,
                                        Total = auditLogs.TotalCount
                                    }
                                }
                            );
                        }
                        catch (Exception ex)
                        {
                            return Results.BadRequest(
                                new ApiResponse<object>
                                {
                                    Error = new ErrorResponse
                                    {
                                        Code = "BadRequest",
                                        Message = ex.Message
                                    }
                                }
                            );
                        }
                    }
                )
                .WithName("GetAuditLogs")
                .RequireAuthorization("AdminOnly")
                .WithTags(AUDITLOG_TAG)
                .WithMetadata();
        }
    }
}
