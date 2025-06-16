using EffortlessQA.Api.Services.Interface;
using EffortlessQA.Data.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace EffortlessQA.Api.Extensions
{
    public static partial class ApiExtensions
    {
        public static void MapTestSuiteEndpoints(this WebApplication app)
        {
            // POST /api/v1/projects/{projectId}/testsuites
            app.MapPost(
                    "/api/v1/projects/{projectId}/testsuites",
                    async (
                        Guid projectId,
                        [FromBody] CreateTestSuiteDto dto,
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
                                tenantId,
                                dto
                            );
                            return Results.Ok(
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

			// GET /api/v1/projects/testsuites
			app.MapGet(
					"/api/v1/projects/testsuites",
					async (
						ITestSuiteService testSuiteService,
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
							var testSuites = await testSuiteService.GetTestSuitesAsync(
								tenantId,
								page,
								limit,
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
				.WithName("GetAllTestSuites")
				.WithTags(TESTSUITE_TAG)
				.WithMetadata();

			// GET /api/v1/projects/{projectId}/testsuites
			app.MapGet(
                    "/api/v1/projects/{projectId}/testsuites",
                    async (
                        Guid projectId,
                        ITestSuiteService testSuiteService,
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
                            var testSuites = await testSuiteService.GetTestSuitesAsync(
                                projectId,
                                tenantId,
                                page,
                                limit,
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
                .WithName("GetTestSuites")
                .WithTags(TESTSUITE_TAG)
                .WithMetadata();

            // GET /api/v1/projects/{projectId}/testsuites/{testSuiteId}
            app.MapGet(
                    "/api/v1/projects/{projectId}/testsuites/{testSuiteId}",
                    async (
                        Guid projectId,
                        Guid testSuiteId,
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
                            var testSuite = await testSuiteService.GetTestSuiteAsync(
                                testSuiteId,
                                projectId,
                                tenantId
                            );
                            return Results.Ok(
                                new ApiResponse<TestSuiteDto>
                                {
                                    Data = testSuite,
                                    Meta = new { Message = "Test suite retrieved successfully" }
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
                .WithName("GetTestSuite")
                .WithTags(TESTSUITE_TAG)
                .WithMetadata();

            // PUT /api/v1/projects/{projectId}/testsuites/{testSuiteId}
            app.MapPut(
                    "/api/v1/projects/{projectId}/testsuites/{testSuiteId}",
                    async (
                        Guid projectId,
                        Guid testSuiteId,
                        [FromBody] UpdateTestSuiteDto dto,
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
                            var testSuite = await testSuiteService.UpdateTestSuiteAsync(
                                testSuiteId,
                                projectId,
                                tenantId,
                                dto
                            );
                            return Results.Ok(
                                new ApiResponse<TestSuiteDto>
                                {
                                    Data = testSuite,
                                    Meta = new { Message = "Test suite updated successfully" }
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
                .WithName("UpdateTestSuite")
                .RequireAuthorization("AdminOnly")
                .WithTags(TESTSUITE_TAG)
                .WithMetadata();

            // DELETE /api/v1/projects/{projectId}/testsuites/{testSuiteId}
            app.MapDelete(
                    "/api/v1/projects/{projectId}/testsuites/{testSuiteId}",
                    async (
                        Guid projectId,
                        Guid testSuiteId,
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
                            await testSuiteService.DeleteTestSuiteAsync(
                                testSuiteId,
                                projectId,
                                tenantId
                            );
                            return Results.Ok(
                                new ApiResponse<object>
                                {
                                    Data = null,
                                    Meta = new { Message = "Test suite deleted successfully" }
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
                .WithName("DeleteTestSuite")
                .RequireAuthorization("AdminOnly")
                .WithTags(TESTSUITE_TAG)
                .WithMetadata();

			// POST /api/v1/projects/{projectId}/requirements/{requirementId}/testsuites
			app.MapPost(
					"/api/v1/projects/{projectId}/requirements/{requirementId}/testsuites",
					async (
						Guid projectId,
						Guid requirementId,
						[FromBody] LinkTestSuiteDto dto,
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
							await testSuiteService.LinkTestSuiteToRequirementAsync(
								requirementId,
								projectId,
								tenantId,
								dto.TestSuiteId
							);
							return Results.Ok(
								new ApiResponse<object>
								{
									Data = null,
									Meta = new
									{
										Message = "Test suite linked to requirement successfully"
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
				.WithName("LinkTestSuiteToRequirement")
				.RequireAuthorization("AdminOnly")
				.WithTags(REQUIREMENT_TAG)
				.WithMetadata();

			// DELETE /api/v1/projects/{projectId}/requirements/{requirementId}/testsuites/{testSuiteId}
			app.MapDelete(
					"/api/v1/projects/{projectId}/requirements/{requirementId}/testsuites/{testSuiteId}",
					async (
						Guid projectId,
						Guid requirementId,
						Guid testSuiteId,
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
							await testSuiteService.UnlinkTestSuiteFromRequirementAsync(
								requirementId,
								projectId,
								tenantId,
								testSuiteId
							);
							return Results.Ok(
								new ApiResponse<object>
								{
									Data = null,
									Meta = new
									{
										Message = "Test suite unlinked from requirement successfully"
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
				.WithName("UnlinkTestSuiteFromRequirement")
				.RequireAuthorization("AdminOnly")
				.WithTags(REQUIREMENT_TAG)
				.WithMetadata();
		}
    }
}
