using EffortlessQA.Api.Services.Interface;
using EffortlessQA.Data.Dtos;
using EffortlessQA.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EffortlessQA.Api.Extensions
{
    public static partial class ApiExtensions
    {
        public static void MapDefectEndpoints(this WebApplication app)
        {
            // POST /api/v1/defects
            app.MapPost(
                    "/api/v1/defects",
                    async (
                        [FromBody] CreateDefectDto dto,
                        IDefectService defectService,
                        HttpContext context
                    ) =>
                    {
                        try
                        {
                            var tenantId = context.User.FindFirst("TenantId")?.Value;
                            if (string.IsNullOrEmpty(tenantId))
                            {
                                return Results.Unauthorized();
                            }
                            var defect = await defectService.CreateDefectAsync(tenantId, dto);
                            return Results.Ok(
                                new ApiResponse<DefectDto>
                                {
                                    Data = defect,
                                    Meta = new { Message = "Defect created successfully" }
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
                .WithName("CreateDefect")
                .RequireAuthorization("TesterOrAdmin")
                .WithTags(DEFECTS_TAG)
                .WithMetadata();

            // GET /api/v1/defects
            app.MapGet(
                    "/api/v1/defects",
                    async (
                        IDefectService defectService,
                        HttpContext context,
                        [FromQuery] int page = 1,
                        [FromQuery] int limit = 50,
                        [FromQuery] string? filter = null,
                        [FromQuery] SeverityLevel[]? severities = null,
                        [FromQuery] DefectStatus[]? statuses = null
                    ) =>
                    {
                        try
                        {
                            var tenantId = context.User.FindFirst("TenantId")?.Value;
                            if (string.IsNullOrEmpty(tenantId))
                            {
                                return Results.Unauthorized();
                            }
                            var defects = await defectService.GetDefectsAsync(
                                tenantId,
                                page,
                                limit,
                                filter,
                                severities,
                                statuses
                            );
                            return Results.Ok(
                                new ApiResponse<PagedResult<DefectDto>>
                                {
                                    Data = defects,
                                    Meta = new
                                    {
                                        Page = page,
                                        Limit = limit,
                                        Total = defects.TotalCount
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
                .WithName("GetDefects")
                .WithTags(DEFECTS_TAG)
                .WithMetadata();

            // GET /api/v1/defects/{defectId}
            app.MapGet(
                    "/api/v1/defects/{defectId}",
                    async (Guid defectId, IDefectService defectService, HttpContext context) =>
                    {
                        try
                        {
                            var tenantId = context.User.FindFirst("TenantId")?.Value;
                            if (string.IsNullOrEmpty(tenantId))
                            {
                                return Results.Unauthorized();
                            }
                            var defect = await defectService.GetDefectAsync(defectId, tenantId);
                            return Results.Ok(
                                new ApiResponse<DefectDto>
                                {
                                    Data = defect,
                                    Meta = new { Message = "Defect retrieved successfully" }
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
                .WithName("GetDefect")
                .WithTags(DEFECTS_TAG)
                .WithMetadata();

            // PUT /api/v1/defects/{defectId}
            app.MapPut(
                    "/api/v1/defects/{defectId}",
                    async (
                        Guid defectId,
                        [FromBody] UpdateDefectDto dto,
                        IDefectService defectService,
                        HttpContext context
                    ) =>
                    {
                        try
                        {
                            var tenantId = context.User.FindFirst("TenantId")?.Value;
                            if (string.IsNullOrEmpty(tenantId))
                            {
                                return Results.Unauthorized();
                            }
                            var defect = await defectService.UpdateDefectAsync(
                                defectId,
                                tenantId,
                                dto
                            );
                            return Results.Ok(
                                new ApiResponse<DefectDto>
                                {
                                    Data = defect,
                                    Meta = new { Message = "Defect updated successfully" }
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
                .WithName("UpdateDefect")
                .RequireAuthorization("TesterOrAdmin")
                .WithTags(DEFECTS_TAG)
                .WithMetadata();

            // DELETE /api/v1/defects/{defectId}
            app.MapDelete(
                    "/api/v1/defects/{defectId}",
                    async (Guid defectId, IDefectService defectService, HttpContext context) =>
                    {
                        try
                        {
                            var tenantId = context.User.FindFirst("TenantId")?.Value;
                            if (string.IsNullOrEmpty(tenantId))
                            {
                                return Results.Unauthorized();
                            }
                            await defectService.DeleteDefectAsync(defectId, tenantId);
                            return Results.Ok(
                                new ApiResponse<object>
                                {
                                    Data = null,
                                    Meta = new { Message = "Defect deleted successfully" }
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
                .WithName("DeleteDefect")
                .RequireAuthorization("AdminOnly")
                .WithTags(DEFECTS_TAG)
                .WithMetadata();

            // GET /api/v1/defects/{defectId}/history
            app.MapGet(
                    "/api/v1/defects/{defectId}/history",
                    async (Guid defectId, IDefectService defectService, HttpContext context) =>
                    {
                        try
                        {
                            var tenantId = context.User.FindFirst("TenantId")?.Value;
                            if (string.IsNullOrEmpty(tenantId))
                            {
                                return Results.Unauthorized();
                            }
                            var history = await defectService.GetDefectHistoryAsync(
                                defectId,
                                tenantId
                            );
                            return Results.Ok(
                                new ApiResponse<IList<DefectHistoryDto>>
                                {
                                    Data = history,
                                    Meta = new { Message = "Defect history retrieved successfully" }
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
                .WithName("GetDefectHistory")
                .WithTags(DEFECTS_TAG)
                .WithMetadata();
        }
    }
}
