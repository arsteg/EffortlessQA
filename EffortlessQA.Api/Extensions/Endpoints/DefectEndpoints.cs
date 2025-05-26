using EffortlessQA.Api.Services.Interface;
using EffortlessQA.Data.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EffortlessQA.Api.Extensions
{
    public static partial class ApiExtensions
    {
        public static void MapDefectEndpoints(this WebApplication app)
        {
            app.MapGet(
                    "/api/v1/defects",
                    async (
                        IDefectService defectService,
                        HttpContext context,
                        [FromQuery] int page = 1,
                        [FromQuery] int limit = 10,
                        [FromQuery] string? sort = "Title",
                        [FromQuery] string? filter = null
                    ) =>
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
                            sort,
                            filter
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
                )
                .WithName("GetDefects")
                .RequireAuthorization("TesterOrAdmin")
                .WithTags(DEFECTS_TAG)
                .WithMetadata();

            app.MapPost(
                    "/api/v1/defects",
                    async (
                        [FromBody] DefectCreateDto dto,
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
                            var defect = await defectService.CreateDefectAsync(dto, tenantId);
                            return Results.Created(
                                $"/api/v1/defects/{defect.Id}",
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
        }
    }
}
