using EffortlessQA.Api.Services.Interface;
using EffortlessQA.Data.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EffortlessQA.Api.Extensions
{
    public static partial class ApiExtensions
    {
        public static void MapTestSuiteEndpoints(this WebApplication app)
        {
            app.MapGet(
                    "/api/v1/projects/{projectId}/testsuites",
                    async (
                        Guid projectId,
                        ITestSuiteService testSuiteService,
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
                        var testSuites = await testSuiteService.GetTestSuitesAsync(
                            projectId,
                            tenantId,
                            page,
                            limit,
                            sort,
                            filter
                        );
                        return Results.Ok(
                            new ApiResponse<PagedResult<TestSuiteDto>>
                            {
                                Data = testSuites,
                                Meta = new
                                {
                                    Page = page,
                                    Limit = limit,
                                    Total = testSuites.TotalCount
                                }
                            }
                        );
                    }
                )
                .WithName("GetTestSuites")
                .RequireAuthorization("TesterOrAdmin")
                .WithTags(TESTSUITE_TAG)
                .WithMetadata();

            app.MapPost(
                    "/api/v1/projects/{projectId}/testsuites",
                    async (
                        Guid projectId,
                        [FromBody] TestSuiteCreateDto dto,
                        ITestSuiteService testSuiteService,
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
                            var testSuite = await testSuiteService.CreateTestSuiteAsync(
                                projectId,
                                dto,
                                tenantId
                            );
                            return Results.Created(
                                $"/api/v1/projects/{projectId}/testsuites/{testSuite.Id}",
                                new ApiResponse<TestSuiteDto>
                                {
                                    Data = testSuite,
                                    Meta = new { Message = "Test suite created successfully" }
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
                .WithName("CreateTestSuite")
                .RequireAuthorization("AdminOnly")
                .WithTags(TESTSUITE_TAG)
                .WithMetadata();
        }
    }
}
