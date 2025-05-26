using EffortlessQA.Data.Dtos;

namespace EffortlessQA.Api.Services.Interface
{
    public interface IPermissionRoleService
    {
        Task<PermissionDto> CreatePermissionAsync(string tenantId, CreatePermissionDto dto);
        Task<PagedResult<PermissionDto>> GetPermissionsAsync(string tenantId, int page, int limit);
        Task<PermissionDto> UpdatePermissionAsync(
            Guid permissionId,
            string tenantId,
            UpdatePermissionDto dto
        );
        Task DeletePermissionAsync(Guid permissionId, string tenantId);
        Task<RoleDto> CreateRoleAsync(Guid projectId, string tenantId, CreateRoleDto dto);
        Task<PagedResult<RoleDto>> GetRolesAsync(
            Guid projectId,
            string tenantId,
            int page,
            int limit
        );
        Task<RoleDto> UpdateRoleAsync(
            Guid projectId,
            Guid roleId,
            string tenantId,
            UpdateRoleDto dto
        );
        Task DeleteRoleAsync(Guid projectId, Guid roleId, string tenantId);
        Task AssignPermissionToRoleAsync(
            Guid projectId,
            Guid roleId,
            Guid permissionId,
            string tenantId
        );
        Task RemovePermissionFromRoleAsync(
            Guid projectId,
            Guid roleId,
            Guid permissionId,
            string tenantId
        );
    }
}
