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
            // POST /api/v1/projects/{projectId}/testruns
            app.MapPost(
                    "/api/v1/projects/{projectId}/testruns",
                    async (
                        Guid projectId,
                        [FromBody] CreateTestRunDto dto,
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
                                tenantId,
                                dto
                            );
                            return Results.Ok(
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

			// GET /api/v1/projects/testruns
			app.MapGet(
					"/api/v1/projects/testruns",
					async (
						ITestRunService testRunService,
						HttpContext context,
						[FromQuery] int page = 1,
						[FromQuery] int limit = 50,
						[FromQuery] string? filter = null,
						[FromQuery] string[]? statuses = null
					) =>
					{
						try
						{
							var tenantId = context.User.FindFirst("TenantId")?.Value;
							if (string.IsNullOrEmpty(tenantId))
							{
								return Results.Unauthorized();
							}
							var testRuns = await testRunService.GetAllTestRunsAsync(
								tenantId,
								page,
								limit,
								filter,
								statuses
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
				.WithName("GetAllTestRuns")
				.WithTags(TESTRUN_TAG)
				.WithMetadata();

			// GET /api/v1/projects/{projectId}/testruns
			app.MapGet(
                    "/api/v1/projects/{projectId}/testruns",
                    async (
                        Guid projectId,
                        ITestRunService testRunService,
                        HttpContext context,
                        [FromQuery] int page = 1,
                        [FromQuery] int limit = 50,
                        [FromQuery] string? filter = null,
                        [FromQuery] string[]? statuses = null
                    ) =>
                    {
                        try
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
                                filter,
                                statuses
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
                .WithName("GetTestRuns")
                .WithTags(TESTRUN_TAG)
                .WithMetadata();

            // GET /api/v1/projects/{projectId}/testruns/{testRunId}
            app.MapGet(
                    "/api/v1/projects/{projectId}/testruns/{testRunId}",
                    async (
                        Guid projectId,
                        Guid testRunId,
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
                            var testRun = await testRunService.GetTestRunAsync(
                                testRunId,
                                projectId,
                                tenantId
                            );
                            return Results.Ok(
                                new ApiResponse<TestRunDto>
                                {
                                    Data = testRun,
                                    Meta = new { Message = "Test run retrieved successfully" }
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
                .WithName("GetTestRun")
                .WithTags(TESTRUN_TAG)
                .WithMetadata();

            // PUT /api/v1/projects/{projectId}/testruns/{testRunId}
            app.MapPut(
                    "/api/v1/projects/{projectId}/testruns/{testRunId}",
                    async (
                        Guid projectId,
                        Guid testRunId,
                        [FromBody] UpdateTestRunDto dto,
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
                            var testRun = await testRunService.UpdateTestRunAsync(
                                testRunId,
                                projectId,
                                tenantId,
                                dto
                            );
                            return Results.Ok(
                                new ApiResponse<TestRunDto>
                                {
                                    Data = testRun,
                                    Meta = new { Message = "Test run updated successfully" }
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
                .WithName("UpdateTestRun")
                .RequireAuthorization("AdminOnly")
                .WithTags(TESTRUN_TAG)
                .WithMetadata();

            // DELETE /api/v1/projects/{projectId}/testruns/{testRunId}
            app.MapDelete(
                    "/api/v1/projects/{projectId}/testruns/{testRunId}",
                    async (
                        Guid projectId,
                        Guid testRunId,
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
                            await testRunService.DeleteTestRunAsync(testRunId, projectId, tenantId);
                            return Results.Ok(
                                new ApiResponse<object>
                                {
                                    Data = null,
                                    Meta = new { Message = "Test run deleted successfully" }
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
                .WithName("DeleteTestRun")
                .RequireAuthorization("AdminOnly")
                .WithTags(TESTRUN_TAG)
                .WithMetadata();
        }
    }
}
