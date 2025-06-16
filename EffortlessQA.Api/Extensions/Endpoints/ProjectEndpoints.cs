using EffortlessQA.Api.Services.Interface;
using EffortlessQA.Data.Dtos;
using EffortlessQA.Data.Dtos.EffortlessQA.Data.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace EffortlessQA.Api.Extensions
{
    public static partial class ApiExtensions
    {
        private const string PROJECT_TAG = "Projects";

        public static void MapProjectEndpoints(this WebApplication app)
        {
            // POST /api/v1/projects
            app.MapPost(
                    "/api/v1/projects",
                    async (
                        [FromBody] CreateProjectDto dto,
                        IProjectService projectService,
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
                            var project = await projectService.CreateProjectAsync(dto, tenantId);
                            return Results.Ok(
                                new ApiResponse<ProjectDto>
                                {
                                    Data = project,
                                    Meta = new { Message = "Project created successfully" }
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
                .WithName("CreateProject")
                .RequireAuthorization("AdminOnly")
                .WithTags(PROJECT_TAG)
                .WithMetadata();

            // GET /api/v1/projects
            app.MapGet(
                    "/api/v1/projects",
                    async (
                        IProjectService projectService,
                        HttpContext context,
                        [FromQuery] int page = 1,
                        [FromQuery] int limit = 50,
                        [FromQuery] string? filter = null
                    ) =>
                    {
                        try
                        {
                            var tenantId = context.User.FindFirst("TenantId")?.Value;
                            if (string.IsNullOrEmpty(tenantId))
                            {
                                return Results.Unauthorized();
                            }
                            var projects = await projectService.GetProjectsAsync(
                                tenantId,
                                page,
                                limit,
                                filter
                            );
                            return Results.Ok(
                                new ApiResponse<PagedResult<ProjectDto>>
                                {
                                    Data = projects,
                                    Meta = new
                                    {
                                        Page = page,
                                        Limit = limit,
                                        Total = projects.TotalCount
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
                .WithName("GetProjects")
                .WithTags(PROJECT_TAG)
                .WithMetadata();

            // GET /api/v1/projects/{projectId}
            app.MapGet(
                    "/api/v1/projects/{projectId}",
                    async (Guid projectId, IProjectService projectService, HttpContext context) =>
                    {
                        try
                        {
                            var tenantId = context.User.FindFirst("TenantId")?.Value;
                            if (string.IsNullOrEmpty(tenantId))
                            {
                                return Results.Unauthorized();
                            }
                            var project = await projectService.GetProjectAsync(projectId, tenantId);
                            return Results.Ok(
                                new ApiResponse<ProjectDto>
                                {
                                    Data = project,
                                    Meta = new { Message = "Project retrieved successfully" }
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
                .WithName("GetProject")
                .WithTags(PROJECT_TAG)
                .WithMetadata();

            // PUT /api/v1/projects/{projectId}
            app.MapPut(
                    "/api/v1/projects/{projectId}",
                    async (
                        Guid projectId,
                        [FromBody] UpdateProjectDto dto,
                        IProjectService projectService,
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
                            var project = await projectService.UpdateProjectAsync(
                                projectId,
                                tenantId,
                                dto
                            );
                            return Results.Ok(
                                new ApiResponse<ProjectDto>
                                {
                                    Data = project,
                                    Meta = new { Message = "Project updated successfully" }
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
                .WithName("UpdateProject")
                .RequireAuthorization("AdminOnly")
                .WithTags(PROJECT_TAG)
                .WithMetadata();

            // DELETE /api/v1/projects/{projectId}
            app.MapDelete(
                    "/api/v1/projects/{projectId}",
                    async (Guid projectId, IProjectService projectService, HttpContext context) =>
                    {
                        try
                        {
                            var tenantId = context.User.FindFirst("TenantId")?.Value;
                            if (string.IsNullOrEmpty(tenantId))
                            {
                                return Results.Unauthorized();
                            }
                            await projectService.DeleteProjectAsync(projectId, tenantId);
                            return Results.Ok(
                                new ApiResponse<object>
                                {
                                    Data = null,
                                    Meta = new { Message = "Project deleted successfully" }
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
                .WithName("DeleteProject")
                .RequireAuthorization("AdminOnly")
                .WithTags(PROJECT_TAG)
                .WithMetadata();

            // POST /api/v1/projects/{projectId}/users
            app.MapPost(
                    "/api/v1/projects/{projectId}/users",
                    async (
                        Guid projectId,
                        [FromBody] AssignUserToProjectDto dto,
                        IProjectService projectService,
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
                            await projectService.AssignUserToProjectAsync(projectId, tenantId, dto);
                            return Results.Ok(
                                new ApiResponse<object>
                                {
                                    Data = null,
                                    Meta = new { Message = "User assigned to project successfully" }
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
                .WithName("AssignUserToProject")
                .RequireAuthorization("AdminOnly")
                .WithTags(PROJECT_TAG)
                .WithMetadata();

            // DELETE /api/v1/projects/{projectId}/users/{userId}
            app.MapDelete(
                    "/api/v1/projects/{projectId}/users/{userId}",
                    async (
                        Guid projectId,
                        Guid userId,
                        IProjectService projectService,
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
                            await projectService.RemoveUserFromProjectAsync(
                                projectId,
                                userId,
                                tenantId
                            );
                            return Results.Ok(
                                new ApiResponse<object>
                                {
                                    Data = null,
                                    Meta = new
                                    {
                                        Message = "User removed from project successfully"
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
                .WithName("RemoveUserFromProject")
                .RequireAuthorization("AdminOnly")
                .WithTags(PROJECT_TAG)
                .WithMetadata();

            app.MapGet(
                    "/api/v1/projects/{projectId}/hierarchy",
                    async (Guid projectId, IProjectService projectService, HttpContext context) =>
                    {
                        try
                        {
                            var tenantId = context.User.FindFirst("TenantId")?.Value;
                            if (string.IsNullOrEmpty(tenantId))
                            {
                                return Results.Unauthorized();
                            }
                            var hierarchy = await projectService.GetProjectHierarchyAsync(
                                projectId
                            );
                            return Results.Ok(
                                new ApiResponse<ProjectHierarchyDto>
                                {
                                    Data = hierarchy,
                                    Meta = new
                                    {
                                        Message = "Project hierarchy retrieved successfully"
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
                .WithName("GetProjectHierarchy")
                .WithTags(PROJECT_TAG)
                .WithMetadata();
        }
    }
}
