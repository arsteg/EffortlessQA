using EffortlessQA.Api.Services.Interface;
using EffortlessQA.Data;
using EffortlessQA.Data.Dtos;
using EffortlessQA.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EffortlessQA.Api.Services.Implementation
{
    public class ProjectService : IProjectService
    {
        private readonly EffortlessQAContext _context;
        private readonly IConfiguration _configuration;

        public ProjectService(EffortlessQAContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<ProjectDto> CreateProjectAsync(CreateProjectDto dto, string tenantId)
        {
            var project = new Project
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                TenantId = tenantId,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };

            await _context.Projects.AddAsync(project);
            await _context.SaveChangesAsync();

            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                TenantId = project.TenantId,
                //CreatedAt = project.CreatedAt,
                //UpdatedAt = project.ModifiedAt
            };
        }

        public async Task<PagedResult<ProjectDto>> GetProjectsAsync(
            string tenantId,
            int page,
            int limit,
            string? filter
        )
        {
            var query = _context.Projects.Where(p => p.TenantId == tenantId && !p.IsDeleted);

            // Parse the filter parameter
            string? nameFilter = null;
            string? sortField = null;
            bool sortAscending = true;

            if (!string.IsNullOrEmpty(filter))
            {
                // Split filter by commas to handle multiple conditions
                var filterConditions = filter
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(f => f.Trim())
                    .ToList();

                foreach (var condition in filterConditions)
                {
                    // Split each condition by colons
                    var filterParts = condition.Split(':');
                    if (filterParts.Length == 3 && filterParts[0].ToLower() == "sort")
                    {
                        sortField = filterParts[1].ToLower();
                        sortAscending = filterParts[2].ToLower() == "asc";
                    }
                    else if (filterParts.Length == 2 && filterParts[0].ToLower() == "name")
                    {
                        nameFilter = filterParts[1];
                    }
                    else
                    {
                        // Fallback: treat the condition as a name search if it doesn't match expected formats
                        nameFilter = condition;
                    }
                }
            }

            // Apply name filter if provided
            if (!string.IsNullOrEmpty(nameFilter))
            {
                query = query.Where(p => p.Name.ToLower().Contains(nameFilter.ToLower()));
            }

            // Apply sorting
            if (sortField == "name")
            {
                query = sortAscending
                    ? query.OrderBy(p => p.Name)
                    : query.OrderByDescending(p => p.Name);
            }
            else
            {
                // Default sorting
                query = query.OrderBy(p => p.Name);
            }

            var totalCount = await query.CountAsync();
            var projects = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    TenantId = p.TenantId
                })
                .ToListAsync();

            return new PagedResult<ProjectDto>
            {
                Items = projects,
                TotalCount = totalCount,
                Page = page,
                Limit = limit
            };
        }

        public async Task<ProjectDto> GetProjectAsync(Guid projectId, string tenantId)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p =>
                p.Id == projectId && p.TenantId == tenantId && !p.IsDeleted
            );

            if (project == null)
                throw new Exception("Project not found.");

            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                TenantId = project.TenantId,
                //CreatedAt = project.CreatedAt,
                //UpdatedAt = project.ModifiedAt
            };
        }

        public async Task<ProjectDto> UpdateProjectAsync(
            Guid projectId,
            string tenantId,
            UpdateProjectDto dto
        )
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p =>
                p.Id == projectId && p.TenantId == tenantId && !p.IsDeleted
            );

            if (project == null)
                throw new Exception("Project not found.");

            project.Name = dto.Name ?? project.Name;
            project.Description = dto.Description ?? project.Description;
            project.ModifiedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                TenantId = project.TenantId,
                //CreatedAt = project.CreatedAt,
                //UpdatedAt = project.ModifiedAt
            };
        }

        public async Task DeleteProjectAsync(Guid projectId, string tenantId)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p =>
                p.Id == projectId && p.TenantId == tenantId && !p.IsDeleted
            );

            if (project == null)
                throw new Exception("Project not found.");

            project.IsDeleted = true;
            project.ModifiedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task AssignUserToProjectAsync(
            Guid projectId,
            string tenantId,
            AssignUserToProjectDto dto
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

            var existingRole = await _context.Roles.FirstOrDefaultAsync(r =>
                r.UserId == dto.UserId && !r.IsDeleted
            );

            if (existingRole != null)
                throw new Exception("User is already assigned to this project.");

            var role = new Role
            {
                UserId = dto.UserId,

                //RoleType = dto.RoleType,
                TenantId = tenantId,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };

            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveUserFromProjectAsync(Guid projectId, Guid userId, string tenantId)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p =>
                p.Id == projectId && p.TenantId == tenantId && !p.IsDeleted
            );

            if (project == null)
                throw new Exception("Project not found.");

            var role = await _context.Roles.FirstOrDefaultAsync(r =>
                r.UserId == userId && !r.IsDeleted
            );

            if (role == null)
                throw new Exception("User is not assigned to this project.");

            role.IsDeleted = true;
            role.ModifiedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
