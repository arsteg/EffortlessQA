using EffortlessQA.Api.Services.Interface;
using EffortlessQA.Data.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EffortlessQA.Api.Extensions
{
    public static partial class ApiExtensions
    {
        public static void MapProjectEndpoints(this WebApplication app)
        {
            app.MapGet(
                    "/api/v1/projects",
                    async (
                        HttpContext context,
                        IProjectService projectService,
                        [FromQuery] int page = 1,
                        [FromQuery] int limit = 10,
                        [FromQuery] string? sort = "Name",
                        [FromQuery] string? filter = null
                    ) =>
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
                            sort,
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
                )
                .WithName("GetProjects")
                .RequireAuthorization("TesterOrAdmin")
                .WithTags(PROJECTS_TAG)
                .WithMetadata();

            app.MapPost(
                    "/api/v1/projects",
                    async (
                        [FromBody] ProjectCreateDto dto,
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
                            return Results.Created(
                                $"/api/v1/projects/{project.Id}",
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
                .WithTags(PROJECTS_TAG)
                .WithMetadata();
        }
    }
}
