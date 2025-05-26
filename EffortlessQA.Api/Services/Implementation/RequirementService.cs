using EffortlessQA.Api.Services.Interface;
using EffortlessQA.Data;
using EffortlessQA.Data.Dtos;
using EffortlessQA.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EffortlessQA.Api.Services.Implementation
{
    public class RequirementService : IRequirementService
    {
        private readonly EffortlessQAContext _context;

        public RequirementService(EffortlessQAContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<RequirementDto>> GetRequirementsAsync(
            Guid projectId,
            string tenantId,
            int page,
            int limit,
            string sort,
            string filter
        )
        {
            var query = _context
                .Requirements.AsNoTracking()
                .Where(r => r.ProjectId == projectId && r.TenantId == tenantId && !r.IsDeleted);

            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(r =>
                    r.Title.Contains(filter) || r.Description.Contains(filter)
                );
            }

            //query = query.OrderBy(sort);
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(r => new RequirementDto
                {
                    Id = r.Id,
                    Title = r.Title,
                    Description = r.Description,
                    Tags = r.Tags,
                    ProjectId = r.ProjectId,
                    TenantId = r.TenantId
                })
                .ToListAsync();

            return new PagedResult<RequirementDto> { Items = items, TotalCount = totalCount };
        }

        public async Task<RequirementDto> CreateRequirementAsync(
            Guid projectId,
            RequirementCreateDto dto,
            string tenantId
        )
        {
            var project = await _context.Projects.FindAsync(projectId);
            if (project == null || project.TenantId != tenantId)
                throw new Exception("Project not found or access denied.");

            var requirement = new Requirement
            {
                Title = dto.Title,
                Description = dto.Description,
                Tags = dto.Tags,
                ProjectId = projectId,
                TenantId = tenantId
            };

            _context.Requirements.Add(requirement);
            await _context.SaveChangesAsync();

            return new RequirementDto
            {
                Id = requirement.Id,
                Title = requirement.Title,
                Description = requirement.Description,
                Tags = requirement.Tags,
                ProjectId = requirement.ProjectId,
                TenantId = requirement.TenantId
            };
        }
    }
}
