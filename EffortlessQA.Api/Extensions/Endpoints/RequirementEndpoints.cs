using EffortlessQA.Api.Services.Interface;
using EffortlessQA.Data.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace EffortlessQA.Api.Extensions
{
    public static partial class ApiExtensions
    {
        public static void MapRequirementEndpoints(this WebApplication app)
        {
            // POST /api/v1/projects/{projectId}/requirements
            app.MapPost(
                    "/api/v1/projects/{projectId}/requirements",
                    async (
                        Guid projectId,
                        [FromBody] CreateRequirementDto dto,
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
                                tenantId,
                                dto
                            );
                            return Results.Ok(
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

            // GET /api/v1/projects/requirements
            app.MapGet(
                    "/api/v1/projects/requirements",
                    async (
                        IRequirementService requirementService,
                        HttpContext context,
                        [FromQuery] int page = 1,
                        [FromQuery] int limit = 50,
                        [FromQuery] string? filter = null,
                        [FromQuery] string[]? tags = null
                    ) =>
                    {
                        try
                        {
                            var tenantId = context.User.FindFirst("TenantId")?.Value;
                            if (string.IsNullOrEmpty(tenantId))
                            {
                                return Results.Unauthorized();
                            }
                            var requirements = await requirementService.GetRequirementsAsync(
                                tenantId,
                                page,
                                limit,
                                filter,
                                tags
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
                .WithName("GetAllRequirements")
                .WithTags(REQUIREMENT_TAG)
                .WithMetadata();

            // GET /api/v1/projects/{projectId}/requirements
            app.MapGet(
                    "/api/v1/projects/{projectId}/requirements",
                    async (
                        Guid projectId,
                        IRequirementService requirementService,
                        HttpContext context,
                        [FromQuery] int page = 1,
                        [FromQuery] int limit = 50,
                        [FromQuery] string? filter = null,
                        [FromQuery] string[]? tags = null
                    ) =>
                    {
                        try
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
                                filter,
                                tags
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
                .WithName("GetRequirements")
                .WithTags(REQUIREMENT_TAG)
                .WithMetadata();

            // GET /api/v1/projects/{projectId}/requirements/{requirementId}
            app.MapGet(
                    "/api/v1/projects/{projectId}/requirements/{requirementId}",
                    async (
                        Guid projectId,
                        Guid requirementId,
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
                            var requirement = await requirementService.GetRequirementAsync(
                                requirementId,
                                projectId,
                                tenantId
                            );
                            return Results.Ok(
                                new ApiResponse<RequirementDto>
                                {
                                    Data = requirement,
                                    Meta = new { Message = "Requirement retrieved successfully" }
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
                .WithName("GetRequirement")
                .WithTags(REQUIREMENT_TAG)
                .WithMetadata();

            // PUT /api/v1/projects/{projectId}/requirements/{requirementId}
            app.MapPut(
                    "/api/v1/projects/{projectId}/requirements/{requirementId}",
                    async (
                        Guid projectId,
                        Guid requirementId,
                        [FromBody] UpdateRequirementDto dto,
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
                            var requirement = await requirementService.UpdateRequirementAsync(
                                requirementId,
                                projectId,
                                tenantId,
                                dto
                            );
                            return Results.Ok(
                                new ApiResponse<RequirementDto>
                                {
                                    Data = requirement,
                                    Meta = new { Message = "Requirement updated successfully" }
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
                .WithName("UpdateRequirement")
                .RequireAuthorization("AdminOnly")
                .WithTags(REQUIREMENT_TAG)
                .WithMetadata();

            // DELETE /api/v1/projects/{projectId}/requirements/{requirementId}
            app.MapDelete(
                    "/api/v1/projects/{projectId}/requirements/{requirementId}",
                    async (
                        Guid projectId,
                        Guid requirementId,
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
                            await requirementService.DeleteRequirementAsync(
                                requirementId,
                                projectId,
                                tenantId
                            );
                            return Results.Ok(
                                new ApiResponse<object>
                                {
                                    Data = null,
                                    Meta = new { Message = "Requirement deleted successfully" }
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
                .WithName("DeleteRequirement")
                .RequireAuthorization("AdminOnly")
                .WithTags(REQUIREMENT_TAG)
                .WithMetadata();

            // POST /api/v1/projects/{projectId}/requirements/{requirementId}/testcases
            app.MapPost(
                    "/api/v1/projects/{projectId}/requirements/{requirementId}/testcases",
                    async (
                        Guid projectId,
                        Guid requirementId,
                        [FromBody] LinkTestCaseDto dto,
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
                            await requirementService.LinkTestCaseToRequirementAsync(
                                requirementId,
                                projectId,
                                tenantId,
                                dto.TestCaseId
                            );
                            return Results.Ok(
                                new ApiResponse<object>
                                {
                                    Data = null,
                                    Meta = new
                                    {
                                        Message = "Test case linked to requirement successfully"
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
                .WithName("LinkTestCaseToRequirement")
                .RequireAuthorization("AdminOnly")
                .WithTags(REQUIREMENT_TAG)
                .WithMetadata();

            // DELETE /api/v1/projects/{projectId}/requirements/{requirementId}/testcases/{testCaseId}
            app.MapDelete(
                    "/api/v1/projects/{projectId}/requirements/{requirementId}/testcases/{testCaseId}",
                    async (
                        Guid projectId,
                        Guid requirementId,
                        Guid testCaseId,
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
                            await requirementService.UnlinkTestCaseFromRequirementAsync(
                                requirementId,
                                projectId,
                                tenantId,
                                testCaseId
                            );
                            return Results.Ok(
                                new ApiResponse<object>
                                {
                                    Data = null,
                                    Meta = new
                                    {
                                        Message = "Test case unlinked from requirement successfully"
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
                .WithName("UnlinkTestCaseFromRequirement")
                .RequireAuthorization("AdminOnly")
                .WithTags(REQUIREMENT_TAG)
                .WithMetadata();
        }
    }
}
