using EffortlessQA.Api.Services.Interface;
using EffortlessQA.Data.Dtos;
using EffortlessQA.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EffortlessQA.Api.Extensions
{
    public static partial class ApiExtensions
    {
        public static void MapTestCaseEndpoints(this WebApplication app)
        {
            // POST /api/v1/testsuites/{testSuiteId}/testcases
            app.MapPost(
                    "/api/v1/testsuites/{testSuiteId}/testcases",
                    async (
                        Guid testSuiteId,
                        [FromBody] CreateTestCaseDto dto,
                        ITestCaseService testCaseService,
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
                            var testCase = await testCaseService.CreateTestCaseAsync(
                                testSuiteId,
                                tenantId,
                                dto
                            );
                            return Results.Ok(
                                new ApiResponse<TestCaseDto>
                                {
                                    Data = testCase,
                                    Meta = new { Message = "Test case created successfully" }
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
                .WithName("CreateTestCase")
                .RequireAuthorization("AdminOnly")
                .WithTags(TESTCASE_TAG)
                .WithMetadata();

			// GET /api/v1/testsuites/{testSuiteId}/testcases
			app.MapGet(
					"/api/v1/testsuites/testcases",
					async (
						ITestCaseService testCaseService,
						HttpContext context,
						[FromQuery] int page = 1,
						[FromQuery] int limit = 50,
						[FromQuery] string? filter = null,
						[FromQuery] string[]? tags = null,
						[FromQuery] PriorityLevel[]? priorities = null
					) =>
					{
						try
						{
							var tenantId = context.User.FindFirst("TenantId")?.Value;
							if (string.IsNullOrEmpty(tenantId))
							{
								return Results.Unauthorized();
							}
							var testCases = await testCaseService.GetTestCasesAsync(
								tenantId,
								page,
								limit,
								filter,
								tags,
								priorities
							);
							return Results.Ok(
								new ApiResponse<PagedResult<TestCaseDto>>
								{
									Data = testCases,
									Meta = new
									{
										Page = page,
										Limit = limit,
										Total = testCases.TotalCount
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
				.WithName("GetAllTestCases")
				.WithTags(TESTCASE_TAG)
				.WithMetadata();

			// GET /api/v1/testsuites/{testSuiteId}/testcases
			app.MapGet(
                    "/api/v1/testsuites/{testSuiteId}/testcases",
                    async (
                        Guid testSuiteId,
                        ITestCaseService testCaseService,
                        HttpContext context,
                        [FromQuery] int page = 1,
                        [FromQuery] int limit = 50,
                        [FromQuery] string? filter = null,
                        [FromQuery] string[]? tags = null,
                        [FromQuery] PriorityLevel[]? priorities = null
                    ) =>
                    {
                        try
                        {
                            var tenantId = context.User.FindFirst("TenantId")?.Value;
                            if (string.IsNullOrEmpty(tenantId))
                            {
                                return Results.Unauthorized();
                            }
                            var testCases = await testCaseService.GetTestCasesAsync(
                                testSuiteId,
                                tenantId,
                                page,
                                limit,
                                filter,
                                tags,
                                priorities
                            );
                            return Results.Ok(
                                new ApiResponse<PagedResult<TestCaseDto>>
                                {
                                    Data = testCases,
                                    Meta = new
                                    {
                                        Page = page,
                                        Limit = limit,
                                        Total = testCases.TotalCount
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
                .WithName("GetTestCases")
                .WithTags(TESTCASE_TAG)
                .WithMetadata();

            // GET /api/v1/testsuites/{testSuiteId}/testcases/{testCaseId}
            app.MapGet(
                    "/api/v1/testsuites/{testSuiteId}/testcases/{testCaseId}",
                    async (
                        Guid testSuiteId,
                        Guid testCaseId,
                        ITestCaseService testCaseService,
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
                            var testCase = await testCaseService.GetTestCaseAsync(
                                testCaseId,
                                testSuiteId,
                                tenantId
                            );
                            return Results.Ok(
                                new ApiResponse<TestCaseDto>
                                {
                                    Data = testCase,
                                    Meta = new { Message = "Test case retrieved successfully" }
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
                .WithName("GetTestCase")
                .WithTags(TESTCASE_TAG)
                .WithMetadata();

            // PUT /api/v1/testsuites/{testSuiteId}/testcases/{testCaseId}
            app.MapPut(
                    "/api/v1/testsuites/{testSuiteId}/testcases/{testCaseId}",
                    async (
                        Guid testSuiteId,
                        Guid testCaseId,
                        [FromBody] UpdateTestCaseDto dto,
                        ITestCaseService testCaseService,
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
                            var testCase = await testCaseService.UpdateTestCaseAsync(
                                testCaseId,
                                testSuiteId,
                                tenantId,
                                dto
                            );
                            return Results.Ok(
                                new ApiResponse<TestCaseDto>
                                {
                                    Data = testCase,
                                    Meta = new { Message = "Test case updated successfully" }
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
                .WithName("UpdateTestCase")
                .RequireAuthorization("AdminOnly")
                .WithTags(TESTCASE_TAG)
                .WithMetadata();

            // DELETE /api/v1/testsuites/{testSuiteId}/testcases/{testCaseId}
            app.MapDelete(
                    "/api/v1/testsuites/{testSuiteId}/testcases/{testCaseId}",
                    async (
                        Guid testSuiteId,
                        Guid testCaseId,
                        ITestCaseService testCaseService,
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
                            await testCaseService.DeleteTestCaseAsync(
                                testCaseId,
                                testSuiteId,
                                tenantId
                            );
                            return Results.Ok(
                                new ApiResponse<object>
                                {
                                    Data = null,
                                    Meta = new { Message = "Test case deleted successfully" }
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
                .WithName("DeleteTestCase")
                .RequireAuthorization("AdminOnly")
                .WithTags(TESTCASE_TAG)
                .WithMetadata();

            // POST /api/v1/testcases/{testCaseId}/copy
            app.MapPost(
                    "/api/v1/testcases/{testCaseId}/copy",
                    async (
                        Guid testCaseId,
                        [FromBody] CopyTestCaseDto dto,
                        ITestCaseService testCaseService,
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
                            var testCase = await testCaseService.CopyTestCaseAsync(
                                testCaseId,
                                tenantId,
                                dto
                            );
                            return Results.Ok(
                                new ApiResponse<TestCaseDto>
                                {
                                    Data = testCase,
                                    Meta = new { Message = "Test case copied successfully" }
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
                .WithName("CopyTestCase")
                .RequireAuthorization("AdminOnly")
                .WithTags(TESTCASE_TAG)
                .WithMetadata();

            // POST /api/v1/testsuites/{testSuiteId}/testcases/import
            app.MapPost(
                    "/api/v1/testsuites/{testSuiteId}/testcases/import",
                    async (
                        Guid testSuiteId,
                        IFormFile file,
                        ITestCaseService testCaseService,
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
                            var testCases = await testCaseService.ImportTestCasesAsync(
                                testSuiteId,
                                tenantId,
                                file
                            );
                            return Results.Ok(
                                new ApiResponse<IList<TestCaseDto>>
                                {
                                    Data = testCases,
                                    Meta = new { Message = "Test cases imported successfully" }
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
                .WithName("ImportTestCases")
                .RequireAuthorization("AdminOnly")
                .WithTags(TESTCASE_TAG)
                .WithMetadata();

            // GET /api/v1/testsuites/{testSuiteId}/testcases/export
            app.MapGet(
                    "/api/v1/testsuites/{testSuiteId}/testcases/export",
                    async (
                        Guid testSuiteId,
                        ITestCaseService testCaseService,
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
                            var csvData = await testCaseService.ExportTestCasesAsync(
                                testSuiteId,
                                tenantId
                            );
                            return Results.File(
                                csvData,
                                contentType: "text/csv",
                                fileDownloadName: $"testcases_{testSuiteId}.csv"
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
                .WithName("ExportTestCases")
                .RequireAuthorization("TesterOrAdmin")
                .WithTags(TESTCASE_TAG)
                .WithMetadata();

            // POST /api/v1/testcases/{testCaseId}/move
            app.MapPost(
                    "/api/v1/testcases/{testCaseId}/move",
                    async (
                        Guid testCaseId,
                        [FromBody] MoveTestCaseDto dto,
                        ITestCaseService testCaseService,
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
                            var testCase = await testCaseService.MoveTestCaseAsync(
                                testCaseId,
                                tenantId,
                                dto
                            );
                            return Results.Ok(
                                new ApiResponse<TestCaseDto>
                                {
                                    Data = testCase,
                                    Meta = new { Message = "Test case moved successfully" }
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
                .WithName("MoveTestCase")
                .RequireAuthorization("AdminOnly")
                .WithTags(TESTCASE_TAG)
                .WithMetadata();
        }
    }
}
