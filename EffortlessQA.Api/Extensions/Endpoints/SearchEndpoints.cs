using EffortlessQA.Api.Services.Interface;
using EffortlessQA.Data.Dtos;
using EffortlessQA.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EffortlessQA.Api.Extensions
{
    public static partial class ApiExtensions
    {
        public static void MapSearchEndpoints(this WebApplication app)
        {
            // GET /api/v1/search
            app.MapGet(
                    "/api/v1/search",
                    async (
                        ISearchService searchService,
                        HttpContext context,
                        [FromQuery] string query,
                        [FromQuery] string[]? tags = null,
                        [FromQuery] int page = 1,
                        [FromQuery] int limit = 50
                    ) =>
                    {
                        try
                        {
                            var tenantId = context.User.FindFirst("TenantId")?.Value;
                            if (string.IsNullOrEmpty(tenantId))
                            {
                                return Results.Unauthorized();
                            }
                            var results = await searchService.GlobalSearchAsync(
                                tenantId,
                                query,
                                tags,
                                page,
                                limit
                            );
                            return Results.Ok(
                                new ApiResponse<PagedResult<SearchResultDto>>
                                {
                                    Data = results,
                                    Meta = new
                                    {
                                        Page = page,
                                        Limit = limit,
                                        Total = results.TotalCount
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
                .WithName("GlobalSearch")
                .WithTags(SEARCH_TAG)
                .WithMetadata();

            // GET /api/v1/projects/{projectId}/filter/requirements
            app.MapGet(
                    "/api/v1/projects/{projectId}/filter/requirements",
                    async (
                        Guid projectId,
                        ISearchService searchService,
                        HttpContext context,
                        [FromQuery] string[]? tags = null,
                        [FromQuery] int page = 1,
                        [FromQuery] int limit = 50
                    ) =>
                    {
                        try
                        {
                            var tenantId = context.User.FindFirst("TenantId")?.Value;
                            if (string.IsNullOrEmpty(tenantId))
                            {
                                return Results.Unauthorized();
                            }
                            var results = await searchService.FilterRequirementsAsync(
                                projectId,
                                tenantId,
                                tags,
                                page,
                                limit
                            );
                            return Results.Ok(
                                new ApiResponse<PagedResult<RequirementDto>>
                                {
                                    Data = results,
                                    Meta = new
                                    {
                                        Page = page,
                                        Limit = limit,
                                        Total = results.TotalCount
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
                .WithName("FilterRequirements")
                .WithTags(SEARCH_TAG)
                .WithMetadata();

            // GET /api/v1/projects/{projectId}/filter/testcases
            app.MapGet(
                    "/api/v1/projects/{projectId}/filter/testcases",
                    async (
                        Guid projectId,
                        ISearchService searchService,
                        HttpContext context,
                        [FromQuery] string[]? tags = null,
                        [FromQuery] PriorityLevel[]? priorities = null,
                        [FromQuery] TestExecutionStatus[]? statuses = null,
                        [FromQuery] int page = 1,
                        [FromQuery] int limit = 50
                    ) =>
                    {
                        try
                        {
                            var tenantId = context.User.FindFirst("TenantId")?.Value;
                            if (string.IsNullOrEmpty(tenantId))
                            {
                                return Results.Unauthorized();
                            }
                            var results = await searchService.FilterTestCasesAsync(
                                projectId,
                                tenantId,
                                tags,
                                priorities,
                                statuses,
                                page,
                                limit
                            );
                            return Results.Ok(
                                new ApiResponse<PagedResult<TestCaseDto>>
                                {
                                    Data = results,
                                    Meta = new
                                    {
                                        Page = page,
                                        Limit = limit,
                                        Total = results.TotalCount
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
                .WithName("FilterTestCases")
                .WithTags(SEARCH_TAG)
                .WithMetadata();

            // GET /api/v1/projects/{projectId}/filter/testruns
            app.MapGet(
                    "/api/v1/projects/{projectId}/filter/testruns",
                    async (
                        Guid projectId,
                        ISearchService searchService,
                        HttpContext context,
                        [FromQuery] TestExecutionStatus[]? statuses = null,
                        [FromQuery] Guid[]? assignedTesterIds = null,
                        [FromQuery] int page = 1,
                        [FromQuery] int limit = 50
                    ) =>
                    {
                        try
                        {
                            var tenantId = context.User.FindFirst("TenantId")?.Value;
                            if (string.IsNullOrEmpty(tenantId))
                            {
                                return Results.Unauthorized();
                            }
                            var results = await searchService.FilterTestRunsAsync(
                                projectId,
                                tenantId,
                                statuses,
                                assignedTesterIds,
                                page,
                                limit
                            );
                            return Results.Ok(
                                new ApiResponse<PagedResult<TestRunDto>>
                                {
                                    Data = results,
                                    Meta = new
                                    {
                                        Page = page,
                                        Limit = limit,
                                        Total = results.TotalCount
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
                .WithName("FilterTestRuns")
                .WithTags(SEARCH_TAG)
                .WithMetadata();

            // GET /api/v1/projects/{projectId}/filter/defects
            app.MapGet(
                    "/api/v1/projects/{projectId}/filter/defects",
                    async (
                        Guid projectId,
                        ISearchService searchService,
                        HttpContext context,
                        [FromQuery] SeverityLevel[]? severities = null,
                        [FromQuery] DefectStatus[]? statuses = null,
                        [FromQuery] int page = 1,
                        [FromQuery] int limit = 50
                    ) =>
                    {
                        try
                        {
                            var tenantId = context.User.FindFirst("TenantId")?.Value;
                            if (string.IsNullOrEmpty(tenantId))
                            {
                                return Results.Unauthorized();
                            }
                            var results = await searchService.FilterDefectsAsync(
                                projectId,
                                tenantId,
                                severities,
                                statuses,
                                page,
                                limit
                            );
                            return Results.Ok(
                                new ApiResponse<PagedResult<DefectDto>>
                                {
                                    Data = results,
                                    Meta = new
                                    {
                                        Page = page,
                                        Limit = limit,
                                        Total = results.TotalCount
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
                .WithName("FilterDefects")
                .WithTags(SEARCH_TAG)
                .WithMetadata();
        }
    }
}
