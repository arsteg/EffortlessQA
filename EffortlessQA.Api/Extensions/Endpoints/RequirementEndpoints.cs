using EffortlessQA.Api.Services.Interface;
using EffortlessQA.Data.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EffortlessQA.Api.Extensions
{
    public static partial class ApiExtensions
    {
        public static void MapRequirementEndpoints(this WebApplication app)
        {
            app.MapGet(
                    "/api/v1/projects/{projectId}/requirements",
                    async (
                        Guid projectId,
                        IRequirementService requirementService,
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
                        var requirements = await requirementService.GetRequirementsAsync(
                            projectId,
                            tenantId,
                            page,
                            limit,
                            sort,
                            filter
                        );
                        return Results.Ok(
                            new ApiResponse<PagedResult<RequirementDto>>
                            {
                                Data = requirements,
                                Meta = new
                                {
                                    Page = page,
                                    Limit = limit,
                                    Total = requirements.TotalCount
                                }
                            }
                        );
                    }
                )
                .WithName("GetRequirements")
                .RequireAuthorization("TesterOrAdmin")
                .WithTags(REQUIREMENT_TAG)
                .WithMetadata();

            app.MapPost(
                    "/api/v1/projects/{projectId}/requirements",
                    async (
                        Guid projectId,
                        [FromBody] RequirementCreateDto dto,
                        IRequirementService requirementService,
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
                            var requirement = await requirementService.CreateRequirementAsync(
                                projectId,
                                dto,
                                tenantId
                            );
                            return Results.Created(
                                $"/api/v1/projects/{projectId}/requirements/{requirement.Id}",
                                new ApiResponse<RequirementDto>
                                {
                                    Data = requirement,
                                    Meta = new { Message = "Requirement created successfully" }
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
                .WithName("CreateRequirement")
                .RequireAuthorization("AdminOnly")
                .WithTags(REQUIREMENT_TAG)
                .WithMetadata();
        }
    }
}
