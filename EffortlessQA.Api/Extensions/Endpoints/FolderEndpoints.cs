using EffortlessQA.Api.Services.Interface;
using EffortlessQA.Data.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace EffortlessQA.Api.Extensions
{
    public static partial class ApiExtensions
    {
        public static void MapTestFolderEndpoints(this WebApplication app)
        {
            // POST /api/v1/projects/{projectId}/testfolders
            app.MapPost(
                    "/api/v1/projects/{projectId}/testfolders",
                    async (
                        Guid projectId,
                        [FromBody] CreateTestFolderDto dto,
                        ITestFolderService testFolderService,
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
                            var testFolder = await testFolderService.CreateTestFolderAsync(
                                projectId,
                                tenantId,
                                dto
                            );
                            return Results.Ok(
                                new ApiResponse<TestFolderDto>
                                {
                                    Data = testFolder,
                                    Meta = new { Message = "Test folder created successfully" }
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
                .WithName("CreateTestFolder")
                .RequireAuthorization("AdminOnly")
                .WithTags(TESTFOLDER_TAG)
                .WithMetadata();

            // GET /api/v1/projects/{projectId}/testfolders
            app.MapGet(
                    "/api/v1/projects/{projectId}/testfolders",
                    async (
                        Guid projectId,
                        ITestFolderService testFolderService,
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
                            var testFolders = await testFolderService.GetTestFoldersAsync(
                                projectId,
                                tenantId,
                                page,
                                limit,
                                filter
                            );
                            return Results.Ok(
                                new ApiResponse<PagedResult<TestFolderDto>>
                                {
                                    Data = testFolders,
                                    Meta = new
                                    {
                                        Page = page,
                                        Limit = limit,
                                        Total = testFolders.TotalCount
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
                .WithName("GetTestFolders")
                .WithTags(TESTFOLDER_TAG)
                .WithMetadata();

            // GET /api/v1/projects/{projectId}/testfolders/{folderId}
            app.MapGet(
                    "/api/v1/projects/{projectId}/testfolders/{folderId}",
                    async (
                        Guid projectId,
                        Guid folderId,
                        ITestFolderService testFolderService,
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
                            var testFolder = await testFolderService.GetTestFolderAsync(
                                folderId,
                                projectId,
                                tenantId
                            );
                            return Results.Ok(
                                new ApiResponse<TestFolderDto>
                                {
                                    Data = testFolder,
                                    Meta = new { Message = "Test folder retrieved successfully" }
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
                .WithName("GetTestFolder")
                .WithTags(TESTFOLDER_TAG)
                .WithMetadata();

            // PUT /api/v1/projects/{projectId}/testfolders/{folderId}
            app.MapPut(
                    "/api/v1/projects/{projectId}/testfolders/{folderId}",
                    async (
                        Guid projectId,
                        Guid folderId,
                        [FromBody] UpdateTestFolderDto dto,
                        ITestFolderService testFolderService,
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
                            var testFolder = await testFolderService.UpdateTestFolderAsync(
                                folderId,
                                projectId,
                                tenantId,
                                dto
                            );
                            return Results.Ok(
                                new ApiResponse<TestFolderDto>
                                {
                                    Data = testFolder,
                                    Meta = new { Message = "Test folder updated successfully" }
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
                .WithName("UpdateTestFolder")
                .RequireAuthorization("AdminOnly")
                .WithTags(TESTFOLDER_TAG)
                .WithMetadata();

            // DELETE /api/v1/projects/{projectId}/testfolders/{folderId}
            app.MapDelete(
                    "/api/v1/projects/{projectId}/testfolders/{folderId}",
                    async (
                        Guid projectId,
                        Guid folderId,
                        ITestFolderService testFolderService,
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
                            await testFolderService.DeleteTestFolderAsync(
                                folderId,
                                projectId,
                                tenantId
                            );
                            return Results.Ok(
                                new ApiResponse<object>
                                {
                                    Data = null,
                                    Meta = new { Message = "Test folder deleted successfully" }
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
                .WithName("DeleteTestFolder")
                .RequireAuthorization("AdminOnly")
                .WithTags(TESTFOLDER_TAG)
                .WithMetadata();
        }
    }
}
