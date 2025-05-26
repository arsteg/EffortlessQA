using EffortlessQA.Api.Services.Interface;
using EffortlessQA.Data.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EffortlessQA.Api.Extensions
{
    public static partial class ApiExtensions
    {
        public static void MapTestRunResultEndpoints(this WebApplication app)
        {
            app.MapPost(
                    "/api/v1/testruns/{testRunId}/results",
                    async (
                        Guid testRunId,
                        [FromBody] TestRunResultCreateDto dto,
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
                            var result = await testRunResultService.CreateTestRunResultAsync(
                                testRunId,
                                dto,
                                tenantId
                            );
                            return Results.Created(
                                $"/api/v1/testruns/{testRunId}/results/{result.Id}",
                                new ApiResponse<TestRunResultDto>
                                {
                                    Data = result,
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
                .WithTags(TESTRUN_RESULT_TAG)
                .WithMetadata();

            app.MapPut(
                    "/api/v1/testruns/{testRunId}/results/bulk",
                    async (
                        Guid testRunId,
                        [FromBody] TestRunResultBulkUpdateDto dto,
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
                            await testRunResultService.BulkUpdateTestRunResultsAsync(
                                testRunId,
                                dto,
                                tenantId
                            );
                            return Results.Ok(
                                new ApiResponse<object>
                                {
                                    Data = null,
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
                .WithTags(TESTRUN_RESULT_TAG)
                .WithMetadata();
        }
    }
}
