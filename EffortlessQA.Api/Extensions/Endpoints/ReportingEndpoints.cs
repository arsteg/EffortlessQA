using EffortlessQA.Api.Services.Interface;
using EffortlessQA.Data.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace EffortlessQA.Api.Extensions
{
    public static partial class ApiExtensions
    {
        public static void MapReportingEndpoints(this WebApplication app)
        {
            // GET /api/v1/projects/{projectId}/dashboard
            app.MapGet(
                    "/api/v1/projects/{projectId}/dashboard",
                    async (
                        Guid projectId,
                        IReportingService reportingService,
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
                            var dashboardData = await reportingService.GetDashboardDataAsync(
                                projectId,
                                tenantId
                            );
                            return Results.Ok(
                                new ApiResponse<DashboardDataDto>
                                {
                                    Data = dashboardData,
                                    Meta = new { Message = "Dashboard data retrieved successfully" }
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
                .WithName("GetDashboard")
                .WithTags(REPORTING_TAG)
                .WithMetadata();

            // GET /api/v1/projects/{projectId}/reports/testrun
            app.MapGet(
                    "/api/v1/projects/{projectId}/reports/testrun",
                    async (
                        Guid projectId,
                        IReportingService reportingService,
                        HttpContext context,
                        [FromQuery] Guid? testRunId = null,
                        [FromQuery] string export = "json"
                    ) =>
                    {
                        try
                        {
                            var tenantId = context.User.FindFirst("TenantId")?.Value;
                            if (string.IsNullOrEmpty(tenantId))
                            {
                                return Results.Unauthorized();
                            }
                            var report = await reportingService.GetTestRunReportAsync(
                                projectId,
                                tenantId,
                                testRunId,
                                export
                            );
                            if (export == "csv")
                            {
                                return Results.File(
                                    report.ExportData,
                                    contentType: report.ExportContentType,
                                    fileDownloadName: report.ExportFileName
                                );
                            }
                            return Results.Ok(
                                new ApiResponse<TestRunReportDto>
                                {
                                    Data = report,
                                    Meta = new
                                    {
                                        Message = "Test run report generated successfully"
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
                .WithName("GetTestRunReport")
                .WithTags(REPORTING_TAG)
                .WithMetadata();

            // GET /api/v1/projects/{projectId}/reports/coverage
            app.MapGet(
                    "/api/v1/projects/{projectId}/reports/coverage",
                    async (
                        Guid projectId,
                        IReportingService reportingService,
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
                            var report = await reportingService.GetCoverageReportAsync(
                                projectId,
                                tenantId
                            );
                            return Results.Ok(
                                new ApiResponse<CoverageReportDto>
                                {
                                    Data = report,
                                    Meta = new
                                    {
                                        Message = "Test coverage report generated successfully"
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
                .WithName("GetCoverageReport")
                .WithTags(REPORTING_TAG)
                .WithMetadata();
        }
    }
}
