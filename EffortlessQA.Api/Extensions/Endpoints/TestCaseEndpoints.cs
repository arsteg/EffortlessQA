//using EffortlessQA.Api.Services.Interface;
//using EffortlessQA.Data.Dtos;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;

//namespace EffortlessQA.Api.Extensions
//{
//    public static partial class ApiExtensions
//    {
//        public static void MapTestCaseEndpoints(this WebApplication app)
//        {
//            app.MapGet(
//                    "/api/v1/testsuites/{testSuiteId}/testcases",
//                    async (
//                        Guid testSuiteId,
//                        ITestCaseService testSuiteService,
//                        HttpContext context,
//                        [FromQuery] int page = 1,
//                        [FromQuery] int limit = 10,
//                        [FromQuery] string? sort = "Title",
//                        [FromQuery] string? filter = null
//                    ) =>
//                    {
//                        var tenantId = context.User.FindFirst("TenantId")?.Value;
//                        if (string.IsNullOrEmpty(tenantId))
//                        {
//                            return Results.Unauthorized();
//                        }
//                        var testCases = await testSuiteService.GetTestCasesAsync(
//                            testSuiteId,
//                            tenantId,
//                            page,
//                            limit,
//                            sort,
//                            filter
//                        );
//                        return Results.Ok(
//                            new ApiResponse<PagedResult<TestCaseDto>>
//                            {
//                                Data = testCases,
//                                Meta = new
//                                {
//                                    Page = page,
//                                    Limit = limit,
//                                    Total = testCases.TotalCount
//                                }
//                            }
//                        );
//                    }
//                )
//                .WithName("GetTestCases")
//                .RequireAuthorization("TesterOrAdmin")
//                .WithTags(TESTCASE_TAG)
//                .WithMetadata();

//            app.MapPost(
//                    "/api/v1/testsuites/{testSuiteId}/testcases",
//                    async (
//                        Guid testSuiteId,
//                        [FromBody] TestCaseCreateDto dto,
//                        ITestSuiteService testSuiteService,
//                        HttpContext context
//                    ) =>
//                    {
//                        try
//                        {
//                            var tenantId = context.User.FindFirst("TenantId")?.Value;
//                            if (string.IsNullOrEmpty(tenantId))
//                            {
//                                return Results.Unauthorized();
//                            }
//                            var testCase = await testSuiteService.CreateTestCaseAsync(
//                                testSuiteId,
//                                dto,
//                                tenantId
//                            );
//                            return Results.Created(
//                                $"/api/v1/testsuites/{testSuiteId}/testcases/{testCase.Id}",
//                                new ApiResponse<TestCaseDto>
//                                {
//                                    Data = testCase,
//                                    Meta = new { Message = "Test case created successfully" }
//                                }
//                            );
//                        }
//                        catch (Exception ex)
//                        {
//                            return Results.BadRequest(
//                                new ApiResponse<object>
//                                {
//                                    Error = new ErrorResponse
//                                    {
//                                        Code = "BadRequest",
//                                        Message = ex.Message
//                                    }
//                                }
//                            );
//                        }
//                    }
//                )
//                .WithName("CreateTestCase")
//                .RequireAuthorization("AdminOnly")
//                .WithTags(TESTCASE_TAG)
//                .WithMetadata();
//        }
//    }
//}
