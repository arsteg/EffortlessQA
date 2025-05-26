using EffortlessQA.Api.Services.Interface;
using EffortlessQA.Data.Dtos;
using EffortlessQA.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EffortlessQA.Api.Extensions
{
    public static partial class ApiExtensions
    {
        private const string TESTRUNRESULT_TAG = "TestRunResults";

        public static void MapTestRunResultEndpoints(this WebApplication app)
        {
            // POST /api/v1/testruns/{testRunId}/results
            app.MapPost(
                    "/api/v1/testruns/{testRunId}/results",
                    async (
                        Guid testRunId,
                        [FromBody] CreateTestRunResultDto dto,
                        ITestRunResultService testRunResultService,
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
                            var testRunResult = await testRunResultService.CreateTestRunResultAsync(
                                testRunId,
                                tenantId,
                                dto
                            );
                            return Results.Ok(
                                new ApiResponse<TestRunResultDto>
                                {
                                    Data = testRunResult,
                                    Meta = new { Message = "Test run result created successfully" }
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
                .WithName("CreateTestRunResult")
                .RequireAuthorization("TesterOrAdmin")
                .WithTags(TESTRUNRESULT_TAG)
                .WithMetadata();

            // GET /api/v1/testruns/{testRunId}/results
            app.MapGet(
                    "/api/v1/testruns/{testRunId}/results",
                    async (
                        Guid testRunId,
                        ITestRunResultService testRunResultService,
                        HttpContext context,
                        [FromQuery] int page = 1,
                        [FromQuery] int limit = 50,
                        [FromQuery] TestExecutionStatus[]? statuses = null
                    ) =>
                    {
                        try
                        {
                            var tenantId = context.User.FindFirst("TenantId")?.Value;
                            if (string.IsNullOrEmpty(tenantId))
                            {
                                return Results.Unauthorized();
                            }
                            var testRunResults = await testRunResultService.GetTestRunResultsAsync(
                                testRunId,
                                tenantId,
                                page,
                                limit,
                                statuses
                            );
                            return Results.Ok(
                                new ApiResponse<PagedResult<TestRunResultDto>>
                                {
                                    Data = testRunResults,
                                    Meta = new
                                    {
                                        Page = page,
                                        Limit = limit,
                                        Total = testRunResults.TotalCount
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
                .WithName("GetTestRunResults")
                .WithTags(TESTRUNRESULT_TAG)
                .WithMetadata();

            // GET /api/v1/testruns/{testRunId}/results/{resultId}
            app.MapGet(
                    "/api/v1/testruns/{testRunId}/results/{resultId}",
                    async (
                        Guid testRunId,
                        Guid resultId,
                        ITestRunResultService testRunResultService,
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
                            var testRunResult = await testRunResultService.GetTestRunResultAsync(
                                resultId,
                                testRunId,
                                tenantId
                            );
                            return Results.Ok(
                                new ApiResponse<TestRunResultDto>
                                {
                                    Data = testRunResult,
                                    Meta = new
                                    {
                                        Message = "Test run result retrieved successfully"
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
                .WithName("GetTestRunResult")
                .WithTags(TESTRUNRESULT_TAG)
                .WithMetadata();

            // PUT /api/v1/testruns/{testRunId}/results/{resultId}
            app.MapPut(
                    "/api/v1/testruns/{testRunId}/results/{resultId}",
                    async (
                        Guid testRunId,
                        Guid resultId,
                        [FromBody] UpdateTestRunResultDto dto,
                        ITestRunResultService testRunResultService,
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
                            var testRunResult = await testRunResultService.UpdateTestRunResultAsync(
                                resultId,
                                testRunId,
                                tenantId,
                                dto
                            );
                            return Results.Ok(
                                new ApiResponse<TestRunResultDto>
                                {
                                    Data = testRunResult,
                                    Meta = new { Message = "Test run result updated successfully" }
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
                .WithName("UpdateTestRunResult")
                .RequireAuthorization("TesterOrAdmin")
                .WithTags(TESTRUNRESULT_TAG)
                .WithMetadata();

            // PUT /api/v1/testruns/{testRunId}/results/bulk
            app.MapPut(
                    "/api/v1/testruns/{testRunId}/results/bulk",
                    async (
                        Guid testRunId,
                        [FromBody] BulkUpdateTestRunResultDto dto,
                        ITestRunResultService testRunResultService,
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
                            var testRunResults =
                                await testRunResultService.BulkUpdateTestRunResultsAsync(
                                    testRunId,
                                    tenantId,
                                    dto
                                );
                            return Results.Ok(
                                new ApiResponse<IList<TestRunResultDto>>
                                {
                                    Data = testRunResults,
                                    Meta = new { Message = "Test run results updated successfully" }
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
                .WithName("BulkUpdateTestRunResults")
                .RequireAuthorization("TesterOrAdmin")
                .WithTags(TESTRUNRESULT_TAG)
                .WithMetadata();
        }
    }
}
