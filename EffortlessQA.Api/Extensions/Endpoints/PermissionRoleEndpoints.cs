using EffortlessQA.Api.Services.Interface;
using EffortlessQA.Data.Dtos;
using EffortlessQA.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EffortlessQA.Api.Extensions
{
    public static partial class ApiExtensions
    {
        public static void MapPermissionRoleEndpoints(this WebApplication app)
        {
            // POST /api/v1/permissions
            app.MapPost(
                    "/api/v1/permissions",
                    async (
                        [FromBody] CreatePermissionDto dto,
                        IPermissionRoleService service,
                        HttpContext context
                    ) =>
                    {
                        try
                        {
                            var tenantId = context.User.FindFirst("TenantId")?.Value;
                            if (string.IsNullOrEmpty(tenantId))
                                return Results.Unauthorized();
                            var permission = await service.CreatePermissionAsync(tenantId, dto);
                            return Results.Ok(
                                new ApiResponse<PermissionDto>
                                {
                                    Data = permission,
                                    Meta = new { Message = "Permission created successfully" }
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
                .WithName("CreatePermission")
                .RequireAuthorization("SuperAdmin")
                .WithTags(PERMISSION_ROLE_TAG)
                .WithMetadata();

            // GET /api/v1/permissions
            app.MapGet(
                    "/api/v1/permissions",
                    async (
                        IPermissionRoleService service,
                        HttpContext context,
                        [FromQuery] int page = 1,
                        [FromQuery] int limit = 50
                    ) =>
                    {
                        try
                        {
                            var tenantId = context.User.FindFirst("TenantId")?.Value;
                            if (string.IsNullOrEmpty(tenantId))
                                return Results.Unauthorized();
                            var permissions = await service.GetPermissionsAsync(
                                tenantId,
                                page,
                                limit
                            );
                            return Results.Ok(
                                new ApiResponse<PagedResult<PermissionDto>>
                                {
                                    Data = permissions,
                                    Meta = new
                                    {
                                        Page = page,
                                        Limit = limit,
                                        Total = permissions.TotalCount
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
                .WithName("GetPermissions")
                .RequireAuthorization("AdminOnly")
                .WithTags(PERMISSION_ROLE_TAG)
                .WithMetadata();

            // PUT /api/v1/permissions/{permissionId}
            app.MapPut(
                    "/api/v1/permissions/{permissionId}",
                    async (
                        Guid permissionId,
                        [FromBody] UpdatePermissionDto dto,
                        IPermissionRoleService service,
                        HttpContext context
                    ) =>
                    {
                        try
                        {
                            var tenantId = context.User.FindFirst("TenantId")?.Value;
                            if (string.IsNullOrEmpty(tenantId))
                                return Results.Unauthorized();
                            var permission = await service.UpdatePermissionAsync(
                                permissionId,
                                tenantId,
                                dto
                            );
                            return Results.Ok(
                                new ApiResponse<PermissionDto>
                                {
                                    Data = permission,
                                    Meta = new { Message = "Permission updated successfully" }
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
                .WithName("UpdatePermission")
                .RequireAuthorization("SuperAdmin")
                .WithTags(PERMISSION_ROLE_TAG)
                .WithMetadata();

            // DELETE /api/v1/permissions/{permissionId}
            app.MapDelete(
                    "/api/v1/permissions/{permissionId}",
                    async (
                        Guid permissionId,
                        IPermissionRoleService service,
                        HttpContext context
                    ) =>
                    {
                        try
                        {
                            var tenantId = context.User.FindFirst("TenantId")?.Value;
                            if (string.IsNullOrEmpty(tenantId))
                                return Results.Unauthorized();
                            await service.DeletePermissionAsync(permissionId, tenantId);
                            return Results.Ok(
                                new ApiResponse<object>
                                {
                                    Data = null,
                                    Meta = new { Message = "Permission deleted successfully" }
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
                .WithName("DeletePermission")
                .RequireAuthorization("SuperAdmin")
                .WithTags(PERMISSION_ROLE_TAG)
                .WithMetadata();

            // POST /api/v1/projects/{projectId}/roles
            app.MapPost(
                    "/api/v1/projects/{projectId}/roles",
                    async (
                        Guid projectId,
                        [FromBody] CreateRoleDto dto,
                        IPermissionRoleService service,
                        HttpContext context
                    ) =>
                    {
                        try
                        {
                            var tenantId = context.User.FindFirst("TenantId")?.Value;
                            if (string.IsNullOrEmpty(tenantId))
                                return Results.Unauthorized();
                            var role = await service.CreateRoleAsync(projectId, tenantId, dto);
                            return Results.Ok(
                                new ApiResponse<RoleDto>
                                {
                                    Data = role,
                                    Meta = new { Message = "Role created successfully" }
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
                .WithName("CreateRole")
                .RequireAuthorization("AdminOnly")
                .WithTags(PERMISSION_ROLE_TAG)
                .WithMetadata();

            // GET /api/v1/projects/{projectId}/roles
            app.MapGet(
                    "/api/v1/projects/{projectId}/roles",
                    async (
                        Guid projectId,
                        IPermissionRoleService service,
                        HttpContext context,
                        [FromQuery] int page = 1,
                        [FromQuery] int limit = 50
                    ) =>
                    {
                        try
                        {
                            var tenantId = context.User.FindFirst("TenantId")?.Value;
                            if (string.IsNullOrEmpty(tenantId))
                            {
                                return Results.Unauthorized();
                            }
                            var roles = await service.GetRolesAsync(
                                projectId,
                                tenantId,
                                page,
                                limit
                            );
                            return Results.Ok(
                                new ApiResponse<PagedResult<RoleDto>>
                                {
                                    Data = roles,
                                    Meta = new
                                    {
                                        Page = page,
                                        Limit = limit,
                                        Total = roles.TotalCount
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
                .WithName("GetRoles")
                .RequireAuthorization("AdminOnly")
                .WithTags(PERMISSION_ROLE_TAG)
                .WithMetadata();

            // PUT /api/v1/projects/{projectId}/roles/{roleId}
            app.MapPut(
                    "/api/v1/projects/{projectId}/roles/{roleId}",
                    async (
                        Guid projectId,
                        Guid roleId,
                        [FromBody] UpdateRoleDto dto,
                        IPermissionRoleService service,
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
                            var role = await service.UpdateRoleAsync(
                                projectId,
                                roleId,
                                tenantId,
                                dto
                            );
                            return Results.Ok(
                                new ApiResponse<RoleDto>
                                {
                                    Data = role,
                                    Meta = new { Message = "Role updated successfully" }
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
                .WithName("UpdateRole")
                .RequireAuthorization("AdminOnly")
                .WithTags(PERMISSION_ROLE_TAG)
                .WithMetadata();

            // DELETE /api/v1/projects/{projectId}/roles/{roleId}
            app.MapDelete(
                    "/api/v1/projects/{projectId}/roles/{roleId}",
                    async (
                        Guid projectId,
                        Guid roleId,
                        IPermissionRoleService service,
                        HttpContext context
                    ) =>
                    {
                        try
                        {
                            var tenantId = context.User.FindFirst("TenantId")?.Value;
                            if (string.IsNullOrEmpty(tenantId))
                                return Results.Unauthorized();
                            await service.DeleteRoleAsync(projectId, roleId, tenantId);
                            return Results.Ok(
                                new ApiResponse<object>
                                {
                                    Data = null,
                                    Meta = new { Message = "Role deleted successfully" }
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
                .WithName("DeleteRole")
                .RequireAuthorization("AdminOnly")
                .WithTags(PERMISSION_ROLE_TAG)
                .WithMetadata();

            // POST /api/v1/projects/{projectId}/roles/{roleId}/permissions
            app.MapPost(
                    "/api/v1/projects/{projectId}/roles/{roleId}/permissions",
                    async (
                        Guid projectId,
                        Guid roleId,
                        [FromBody] AssignPermissionDto dto,
                        IPermissionRoleService service,
                        HttpContext context
                    ) =>
                    {
                        try
                        {
                            var tenantId = context.User.FindFirst("TenantId")?.Value;
                            if (string.IsNullOrEmpty(tenantId))
                                return Results.Unauthorized();
                            await service.AssignPermissionToRoleAsync(
                                projectId,
                                roleId,
                                dto.PermissionId,
                                tenantId
                            );
                            return Results.Ok(
                                new ApiResponse<object>
                                {
                                    Data = null,
                                    Meta = new
                                    {
                                        Message = "Permission assigned to role successfully"
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
                .WithName("AssignPermissionToRole")
                .RequireAuthorization("AdminOnly")
                .WithTags(PERMISSION_ROLE_TAG)
                .WithMetadata();

            // DELETE /api/v1/projects/{projectId}/roles/{roleId}/permissions/{permissionId}
            app.MapDelete(
                    "/api/v1/projects/{projectId}/roles/{roleId}/permissions/{permissionId}",
                    async (
                        Guid projectId,
                        Guid roleId,
                        Guid permissionId,
                        IPermissionRoleService service,
                        HttpContext context
                    ) =>
                    {
                        try
                        {
                            var tenantId = context.User.FindFirst("TenantId")?.Value;
                            if (string.IsNullOrEmpty(tenantId))
                                return Results.Unauthorized();
                            await service.RemovePermissionFromRoleAsync(
                                projectId,
                                roleId,
                                permissionId,
                                tenantId
                            );
                            return Results.Ok(
                                new ApiResponse<object>
                                {
                                    Data = null,
                                    Meta = new
                                    {
                                        Message = "Permission removed from role successfully"
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
                .WithName("RemovePermissionFromRole")
                .RequireAuthorization("AdminOnly")
                .WithTags(PERMISSION_ROLE_TAG)
                .WithMetadata();
        }
    }
}
