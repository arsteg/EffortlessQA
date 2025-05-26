using EffortlessQA.Api.Services.Interface;
using EffortlessQA.Data.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EffortlessQA.Api.Extensions
{
    public static partial class ApiExtensions
    {
        public static void MapTestRunEndpoints(this WebApplication app)
        {
            app.MapGet(
                    "/api/v1/projects/{projectId}/testruns",
                    async (
                        Guid projectId,
                        ITestRunService testRunService,
                        HttpContext context,
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
                        var testRuns = await testRunService.GetTestRunsAsync(
                            projectId,
                            tenantId,
                            page,
                            limit,
                            sort,
                            filter
                        );
                        return Results.Ok(
                            new ApiResponse<PagedResult<TestRunDto>>
                            {
                                Data = testRuns,
                                Meta = new
                                {
                                    Page = page,
                                    Limit = limit,
                                    Total = testRuns.TotalCount
                                }
                            }
                        );
                    }
                )
                .WithName("GetTestRuns")
                .RequireAuthorization("TesterOrAdmin")
                .WithTags(TESTRUN_TAG)
                .WithMetadata();

            app.MapPost(
                    "/api/v1/projects/{projectId}/testruns",
                    async (
                        Guid projectId,
                        [FromBody] TestRunCreateDto dto,
                        ITestRunService testRunService,
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
                            var testRun = await testRunService.CreateTestRunAsync(
                                projectId,
                                dto,
                                tenantId
                            );
                            return Results.Created(
                                $"/api/v1/projects/{projectId}/testruns/{testRun.Id}",
                                new ApiResponse<TestRunDto>
                                {
                                    Data = testRun,
                                    Meta = new { Message = "Test run created successfully" }
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
                .WithName("CreateTestRun")
                .RequireAuthorization("AdminOnly")
                .WithTags(TESTRUN_TAG)
                .WithMetadata();
        }
    }
}
