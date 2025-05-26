using EffortlessQA.Api.Services.Interface;
using EffortlessQA.Data.Dtos;
using Microsoft.AspNetCore.Authorization; // For RequireAuthorization
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EffortlessQA.Api.Extensions
{
    public static class ApiExtensions
    {
        public static void MapApiEndpoints(this WebApplication app)
        {
            app.MapPost(
                    "/api/v1/auth/register",
                    async ([FromBody] RegisterDto dto, IAuthService authService) =>
                    {
                        try
                        {
                            var result = await authService.RegisterAsync(dto);
                            return Results.Ok(new ApiResponse<UserDto> { Data = result });
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
                .WithName("Register")
                .WithMetadata();

            app.MapPost(
                    "/api/v1/auth/login",
                    async ([FromBody] LoginDto dto, IAuthService authService) =>
                    {
                        try
                        {
                            var token = await authService.LoginAsync(dto);
                            return Results.Ok(new ApiResponse<string> { Data = token });
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
                .WithName("Login")
                .WithMetadata();

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
                            var project = await projectService.CreateProjectAsync(dto, tenantId);
                            return Results.Created(
                                $"/api/v1/projects/{project.Id}",
                                new ApiResponse<ProjectDto> { Data = project }
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
                .WithMetadata();

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
                            var testSuite = await testSuiteService.CreateTestSuiteAsync(
                                projectId,
                                dto,
                                tenantId
                            );
                            return Results.Created(
                                $"/api/v1/projects/{projectId}/testsuites/{testSuite.Id}",
                                new ApiResponse<TestSuiteDto> { Data = testSuite }
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
                .WithMetadata();

            app.MapGet(
                    "/api/v1/testsuites/{testSuiteId}/testcases",
                    async (
                        Guid testSuiteId,
                        ITestSuiteService testSuiteService,
                        HttpContext context,
                        [FromQuery] int page = 1,
                        [FromQuery] int limit = 10,
                        [FromQuery] string? sort = "Title",
                        [FromQuery] string? filter = null
                    ) =>
                    {
                        var tenantId = context.User.FindFirst("TenantId")?.Value;
                        var testCases = await testSuiteService.GetTestCasesAsync(
                            testSuiteId,
                            tenantId,
                            page,
                            limit,
                            sort,
                            filter
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
                )
                .WithName("GetTestCases")
                .RequireAuthorization("TesterOrAdmin")
                .WithMetadata();

            app.MapPost(
                    "/api/v1/testsuites/{testSuiteId}/testcases",
                    async (
                        Guid testSuiteId,
                        [FromBody] TestCaseCreateDto dto,
                        ITestSuiteService testSuiteService,
                        HttpContext context
                    ) =>
                    {
                        try
                        {
                            var tenantId = context.User.FindFirst("TenantId")?.Value;
                            var testCase = await testSuiteService.CreateTestCaseAsync(
                                testSuiteId,
                                dto,
                                tenantId
                            );
                            return Results.Created(
                                $"/api/v1/testsuites/{testSuiteId}/testcases/{testCase.Id}",
                                new ApiResponse<TestCaseDto> { Data = testCase }
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
                .WithMetadata();

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
                            var testRun = await testRunService.CreateTestRunAsync(
                                projectId,
                                dto,
                                tenantId
                            );
                            return Results.Created(
                                $"/api/v1/projects/{projectId}/testruns/{testRun.Id}",
                                new ApiResponse<TestRunDto> { Data = testRun }
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
                .WithMetadata();

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
                            var result = await testRunResultService.CreateTestRunResultAsync(
                                testRunId,
                                dto,
                                tenantId
                            );
                            return Results.Created(
                                $"/api/v1/testruns/{testRunId}/results/{result.Id}",
                                new ApiResponse<TestRunResultDto> { Data = result }
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
                            await testRunResultService.BulkUpdateTestRunResultsAsync(
                                testRunId,
                                dto,
                                tenantId
                            );
                            return Results.Ok(new ApiResponse<object> { Data = null });
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
                .WithMetadata();

            app.MapGet(
                    "/api/v1/defects",
                    async (
                        IDefectService defectService,
                        HttpContext context,
                        [FromQuery] int page = 1,
                        [FromQuery] int limit = 10,
                        [FromQuery] string? sort = "Title",
                        [FromQuery] string? filter = null
                    ) =>
                    {
                        var tenantId = context.User.FindFirst("TenantId")?.Value;
                        var defects = await defectService.GetDefectsAsync(
                            tenantId,
                            page,
                            limit,
                            sort,
                            filter
                        );
                        return Results.Ok(
                            new ApiResponse<PagedResult<DefectDto>>
                            {
                                Data = defects,
                                Meta = new
                                {
                                    Page = page,
                                    Limit = limit,
                                    Total = defects.TotalCount
                                }
                            }
                        );
                    }
                )
                .WithName("GetDefects")
                .RequireAuthorization("TesterOrAdmin")
                .WithMetadata();

            app.MapPost(
                    "/api/v1/defects",
                    async (
                        [FromBody] DefectCreateDto dto,
                        IDefectService defectService,
                        HttpContext context
                    ) =>
                    {
                        try
                        {
                            var tenantId = context.User.FindFirst("TenantId")?.Value;
                            var defect = await defectService.CreateDefectAsync(dto, tenantId);
                            return Results.Created(
                                $"/api/v1/defects/{defect.Id}",
                                new ApiResponse<DefectDto> { Data = defect }
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
                .WithName("CreateDefect")
                .RequireAuthorization("TesterOrAdmin")
                .WithMetadata();

            app.MapGet(
                    "/api/v1/projects/{projectId}/requirements",
                    async (
                        Guid projectId,
                        IRequirementService requirementService,
                        HttpContext context,
                        [FromQuery] int page = 1,
                        [FromQuery] int limit = 10,
                        [FromQuery] string? sort = "Title",
                        [FromQuery] string? filter = null
                    ) =>
                    {
                        var tenantId = context.User.FindFirst("TenantId")?.Value;
                        var requirements = await requirementService.GetRequirementsAsync(
                            projectId,
                            tenantId,
                            page,
                            limit,
                            sort,
                            filter
                        );
                        return Results.Ok(
                            new ApiResponse<PagedResult<RequirementDto>>
                            {
                                Data = requirements,
                                Meta = new
                                {
                                    Page = page,
                                    Limit = limit,
                                    Total = requirements.TotalCount
                                }
                            }
                        );
                    }
                )
                .WithName("GetRequirements")
                .RequireAuthorization("TesterOrAdmin")
                .WithMetadata();

            app.MapPost(
                    "/api/v1/projects/{projectId}/requirements",
                    async (
                        Guid projectId,
                        [FromBody] RequirementCreateDto dto,
                        IRequirementService requirementService,
                        HttpContext context
                    ) =>
                    {
                        try
                        {
                            var tenantId = context.User.FindFirst("TenantId")?.Value;
                            var requirement = await requirementService.CreateRequirementAsync(
                                projectId,
                                dto,
                                tenantId
                            );
                            return Results.Created(
                                $"/api/v1/projects/{projectId}/requirements/{requirement.Id}",
                                new ApiResponse<RequirementDto> { Data = requirement }
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
                .WithName("CreateRequirement")
                .RequireAuthorization("AdminOnly")
                .WithMetadata();
        }
    }
}
