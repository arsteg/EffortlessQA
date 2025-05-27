using EffortlessQA.Api.Services.Interface;
using EffortlessQA.Data;
using EffortlessQA.Data.Dtos;
using EffortlessQA.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EffortlessQA.Api.Services.Implementation
{
    public class PermissionRoleService : IPermissionRoleService
    {
        private readonly EffortlessQAContext _context;
        private readonly IConfiguration _configuration;

        public PermissionRoleService(EffortlessQAContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<PermissionDto> CreatePermissionAsync(
            string tenantId,
            CreatePermissionDto dto
        )
        {
            var existingPermission = await _context.Permissions.FirstOrDefaultAsync(p =>
                p.Name == dto.Name && !p.IsDeleted
            );
            if (existingPermission != null)
                throw new Exception("Permission with this name already exists.");

            var permission = new Permission
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };

            await _context.Permissions.AddAsync(permission);
            await _context.SaveChangesAsync();

            return new PermissionDto
            {
                Id = permission.Id,
                Name = permission.Name,
                Description = permission.Description,
                CreatedAt = permission.CreatedAt,
                //UpdatedAt = permission.ModifiedAt
            };
        }

        public async Task<PagedResult<PermissionDto>> GetPermissionsAsync(
            string tenantId,
            int page,
            int limit
        )
        {
            var query = _context.Permissions.Where(p => !p.IsDeleted).OrderBy(p => p.Name);

            var totalCount = await query.CountAsync();
            var permissions = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(p => new PermissionDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    CreatedAt = p.CreatedAt,
                    //UpdatedAt = p.ModifiedAt
                })
                .ToListAsync();

            return new PagedResult<PermissionDto>
            {
                Items = permissions,
                TotalCount = totalCount,
                Page = page,
                Limit = limit
            };
        }

        public async Task<PermissionDto> UpdatePermissionAsync(
            Guid permissionId,
            string tenantId,
            UpdatePermissionDto dto
        )
        {
            var permission = await _context.Permissions.FirstOrDefaultAsync(p =>
                p.Id == permissionId && !p.IsDeleted
            );

            if (permission == null)
                throw new Exception("Permission not found.");

            if (!string.IsNullOrEmpty(dto.Name) && dto.Name != permission.Name)
            {
                var existingPermission = await _context.Permissions.FirstOrDefaultAsync(p =>
                    p.Name == dto.Name && !p.IsDeleted
                );
                if (existingPermission != null)
                    throw new Exception("Permission with this name already exists.");
                permission.Name = dto.Name;
            }

            permission.Description = dto.Description ?? permission.Description;
            permission.ModifiedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new PermissionDto
            {
                Id = permission.Id,
                Name = permission.Name,
                Description = permission.Description,
                CreatedAt = permission.CreatedAt,
                //UpdatedAt = permission.ModifiedAt
            };
        }

        public async Task DeletePermissionAsync(Guid permissionId, string tenantId)
        {
            var permission = await _context.Permissions.FirstOrDefaultAsync(p =>
                p.Id == permissionId && !p.IsDeleted
            );

            if (permission == null)
                throw new Exception("Permission not found.");

            permission.IsDeleted = true;
            permission.ModifiedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task<RoleDto> CreateRoleAsync(
            Guid projectId,
            string tenantId,
            CreateRoleDto dto
        )
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p =>
                p.Id == projectId && p.TenantId == tenantId && !p.IsDeleted
            );
            if (project == null)
                throw new Exception("Project not found.");

            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Id == dto.UserId && u.TenantId == tenantId && !u.IsDeleted
            );
            if (user == null)
                throw new Exception("User not found.");

            var role = new Role
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                //ProjectId = projectId,
                RoleType = dto.RoleType,
                TenantId = tenantId,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };

            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();

            return new RoleDto
            {
                Id = role.Id,
                UserId = role.UserId,
                // ProjectId = role.ProjectId,
                //RoleType = role.RoleType,
                //TenantId = role.TenantId,
                CreatedAt = role.CreatedAt,
                //UpdatedAt = role.ModifiedAt
            };
        }

        public async Task<PagedResult<RoleDto>> GetRolesAsync(
            Guid projectId,
            string tenantId,
            int page,
            int limit
        )
        {
            var query = _context
                .Roles.Where(r => r.TenantId == tenantId && !r.IsDeleted)
                .OrderBy(r => r.RoleType);

            var totalCount = await query.CountAsync();
            var roles = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(r => new RoleDto
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    // ProjectId = r.ProjectId,
                    // RoleType = r.RoleType,
                    //TenantId = r.TenantId,
                    CreatedAt = r.CreatedAt,
                    //UpdatedAt = r.ModifiedAt
                })
                .ToListAsync();

            return new PagedResult<RoleDto>
            {
                Items = roles,
                TotalCount = totalCount,
                Page = page,
                Limit = limit
            };
        }

        public async Task<RoleDto> UpdateRoleAsync(
            Guid projectId,
            Guid roleId,
            string tenantId,
            UpdateRoleDto dto
        )
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r =>
                r.Id == roleId && r.TenantId == tenantId && !r.IsDeleted
            );

            if (role == null)
                throw new Exception("Role not found.");

            if (dto.UserId.HasValue)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u =>
                    u.Id == dto.UserId && u.TenantId == tenantId && !u.IsDeleted
                );
                if (user == null)
                    throw new Exception("User not found.");
                role.UserId = dto.UserId.Value;
            }

            if (dto.RoleType.HasValue)
            {
                role.RoleType = dto.RoleType.Value;
            }

            role.ModifiedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return new RoleDto
            {
                Id = role.Id,
                UserId = role.UserId,
                //ProjectId = role.ProjectId,
                //RoleType = role.RoleType,
                // TenantId = role.TenantId,
                //CreatedAt = role.CreatedAt,
                //UpdatedAt = role.ModifiedAt
            };
        }

        public async Task DeleteRoleAsync(Guid projectId, Guid roleId, string tenantId)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r =>
                r.Id == roleId && r.TenantId == tenantId && !r.IsDeleted
            );

            if (role == null)
                throw new Exception("Role not found.");

            role.IsDeleted = true;
            role.ModifiedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task AssignPermissionToRoleAsync(
            Guid projectId,
            Guid roleId,
            Guid permissionId,
            string tenantId
        )
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r =>
                r.Id == roleId && r.TenantId == tenantId && !r.IsDeleted
            );
            if (role == null)
                throw new Exception("Role not found.");

            var permission = await _context.Permissions.FirstOrDefaultAsync(p =>
                p.Id == permissionId && !p.IsDeleted
            );
            if (permission == null)
                throw new Exception("Permission not found.");

            var existingRolePermission = await _context.RolePermissions.FirstOrDefaultAsync(rp =>
                rp.RoleId == roleId && rp.PermissionId == permissionId && !rp.IsDeleted
            );
            if (existingRolePermission != null)
                throw new Exception("Permission already assigned to this role.");

            var rolePermission = new RolePermission
            {
                RoleId = roleId,
                PermissionId = permissionId,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };

            await _context.RolePermissions.AddAsync(rolePermission);
            await _context.SaveChangesAsync();
        }

        public async Task RemovePermissionFromRoleAsync(
            Guid projectId,
            Guid roleId,
            Guid permissionId,
            string tenantId
        )
        {
            var rolePermission = await _context.RolePermissions.FirstOrDefaultAsync(rp =>
                rp.RoleId == roleId
                && rp.PermissionId == permissionId
                && !rp.IsDeleted
                // && rp.Role.ProjectId == projectId
                && rp.Role.TenantId == tenantId
            );

            if (rolePermission == null)
                throw new Exception("Role permission not found.");

            rolePermission.IsDeleted = true;
            rolePermission.ModifiedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
