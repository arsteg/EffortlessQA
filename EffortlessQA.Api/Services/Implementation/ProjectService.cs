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

        public ProjectService(EffortlessQAContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<ProjectDto>> GetProjectsAsync(
            string tenantId,
            int page,
            int limit,
            string sort,
            string filter
        )
        {
            var query = _context
                .Projects.AsNoTracking()
                .Where(p => p.TenantId == tenantId && !p.IsDeleted);

            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(p => p.Name.Contains(filter) || p.Description.Contains(filter));
            }

            //query = query.OrderBy(sort);
            var totalCount = await query.CountAsync();
            var items = await query
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

            return new PagedResult<ProjectDto> { Items = items, TotalCount = totalCount };
        }

        public async Task<ProjectDto> CreateProjectAsync(ProjectCreateDto dto, string tenantId)
        {
            var project = new Project
            {
                Name = dto.Name,
                Description = dto.Description,
                TenantId = tenantId
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                TenantId = project.TenantId
            };
        }
    }
}
