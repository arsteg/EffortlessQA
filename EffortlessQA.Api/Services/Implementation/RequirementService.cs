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
        private readonly IConfiguration _configuration;

        public RequirementService(EffortlessQAContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<RequirementDto> CreateRequirementAsync(
            Guid projectId,
            string tenantId,
            CreateRequirementDto dto
        )
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p =>
                p.Id == projectId && p.TenantId == tenantId && !p.IsDeleted
            );

            if (project == null)
                throw new Exception("Project not found.");

            var requirement = new Requirement
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                Tags = dto.Tags,
                ProjectId = projectId,
                TenantId = tenantId,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
                ParentRequirementId = dto.ParentRequirementId
            };

            await _context.Requirements.AddAsync(requirement);
            await _context.SaveChangesAsync();

            return new RequirementDto
            {
                Id = requirement.Id,
                Title = requirement.Title,
                Description = requirement.Description,
                Tags = requirement.Tags,
                ProjectId = requirement.ProjectId,
                TenantId = requirement.TenantId,
                CreatedAt = requirement.CreatedAt,
                UpdatedAt = requirement.ModifiedAt,
                ParentRequirementId = requirement.ParentRequirementId
            };
        }

        public async Task<PagedResult<RequirementDto>> GetRequirementsAsync(
            string tenantId,
            int page,
            int limit,
            string? filter,
            string[]? tags
        )
        {
            // Build query for top-level requirements
            var query = _context.Requirements.Where(r =>
                r.TenantId == tenantId && !r.IsDeleted && r.ParentRequirementId == null
            );

            // Parse filter parameter
            string? titleFilter = null;
            string? sortField = null;
            bool sortAscending = true;

            if (!string.IsNullOrEmpty(filter))
            {
                var filterConditions = filter
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(f => f.Trim())
                    .ToList();

                foreach (var condition in filterConditions)
                {
                    var filterParts = condition.Split(':');
                    if (filterParts.Length == 3 && filterParts[0].ToLower() == "sort")
                    {
                        sortField = filterParts[1].ToLower();
                        sortAscending = filterParts[2].ToLower() == "asc";
                    }
                    else if (filterParts.Length == 2 && filterParts[0].ToLower() == "title")
                    {
                        titleFilter = filterParts[1];
                    }
                    else
                    {
                        titleFilter = condition;
                    }
                }
            }

            // Apply title filter (case-insensitive)
            if (!string.IsNullOrEmpty(titleFilter))
            {
                query = query.Where(r => r.Title.ToLower().Contains(titleFilter.ToLower()));
            }

            // Apply tags filter
            if (tags != null && tags.Length > 0)
            {
                query = query.Where(r => r.Tags != null && tags.Any(t => r.Tags.Contains(t)));
            }

            // Apply sorting
            if (sortField == "title")
            {
                query = sortAscending
                    ? query.OrderBy(r => r.Title)
                    : query.OrderByDescending(r => r.Title);
            }
            else if (sortField == "description")
            {
                query = sortAscending
                    ? query.OrderBy(r => r.Description)
                    : query.OrderByDescending(r => r.Description);
            }
            else if (sortField == "createdat")
            {
                query = sortAscending
                    ? query.OrderBy(r => r.CreatedAt)
                    : query.OrderByDescending(r => r.CreatedAt);
            }
            else if (sortField == "updatedat")
            {
                query = sortAscending
                    ? query.OrderBy(r => r.ModifiedAt)
                    : query.OrderByDescending(r => r.ModifiedAt);
            }
            else
            {
                query = query.OrderBy(r => r.Title);
            }

            // Get total count of top-level requirements
            var totalCount = await query.CountAsync();

            // Fetch top-level requirements with pagination
            var topLevelRequirements = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            // Fetch all requirements to build hierarchy
            var allRequirements = await _context
                .Requirements.Where(r => r.TenantId == tenantId && !r.IsDeleted)
                .ToListAsync();

            // Map to DTOs with children
            var requirementDtos = topLevelRequirements
                .Select(r => MapToDto(r, allRequirements))
                .ToList();

            return new PagedResult<RequirementDto>
            {
                Items = requirementDtos,
                TotalCount = totalCount,
                Page = page,
                Limit = limit
            };
        }

        public async Task<PagedResult<RequirementDto>> GetRequirementsAsync(
            Guid projectId,
            string tenantId,
            int page,
            int limit,
            string? filter,
            string[]? tags
        )
        {
            // Build query for top-level requirements
            var query = _context.Requirements.Where(r =>
                r.ProjectId == projectId
                && r.TenantId == tenantId
                && !r.IsDeleted
                && r.ParentRequirementId == null
            );

            // Apply title filter
            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(r => r.Title.ToLower().Contains(filter.ToLower()));
            }

            // Apply tags filter
            if (tags != null && tags.Length > 0)
            {
                query = query.Where(r => r.Tags != null && tags.Any(t => r.Tags.Contains(t)));
            }

            // Apply default sorting
            query = query.OrderBy(r => r.Title);

            // Get total count of top-level requirements
            var totalCount = await query.CountAsync();

            // Fetch top-level requirements with pagination
            var topLevelRequirements = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            // Fetch all requirements for the project to build hierarchy
            var allRequirements = await _context
                .Requirements.Where(r =>
                    r.ProjectId == projectId && r.TenantId == tenantId && !r.IsDeleted
                )
                .ToListAsync();

            // Map to DTOs with children
            var requirementDtos = topLevelRequirements
                .Select(r => MapToDto(r, allRequirements))
                .ToList();

            return new PagedResult<RequirementDto>
            {
                Items = requirementDtos,
                TotalCount = totalCount,
                Page = page,
                Limit = limit
            };
        }

        public async Task<RequirementDto> GetRequirementAsync(
            Guid requirementId,
            Guid projectId,
            string tenantId
        )
        {
            var requirement = await _context.Requirements.FirstOrDefaultAsync(r =>
                r.Id == requirementId
                && r.ProjectId == projectId
                && r.TenantId == tenantId
                && !r.IsDeleted
            );

            if (requirement == null)
                throw new Exception("Requirement not found.");

            return new RequirementDto
            {
                Id = requirement.Id,
                Title = requirement.Title,
                Description = requirement.Description,
                Tags = requirement.Tags,
                ProjectId = requirement.ProjectId,
                TenantId = requirement.TenantId,
                CreatedAt = requirement.CreatedAt,
                UpdatedAt = requirement.ModifiedAt
            };
        }

        public async Task<RequirementDto> UpdateRequirementAsync(
            Guid requirementId,
            Guid projectId,
            string tenantId,
            UpdateRequirementDto dto
        )
        {
            var requirement = await _context.Requirements.FirstOrDefaultAsync(r =>
                r.Id == requirementId
                && r.ProjectId == projectId
                && r.TenantId == tenantId
                && !r.IsDeleted
            );

            if (requirement == null)
                throw new Exception("Requirement not found.");

            requirement.Title = dto.Title ?? requirement.Title;
            requirement.Description = dto.Description ?? requirement.Description;
            requirement.Tags = dto.Tags ?? requirement.Tags;
            requirement.ModifiedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new RequirementDto
            {
                Id = requirement.Id,
                Title = requirement.Title,
                Description = requirement.Description,
                Tags = requirement.Tags,
                ProjectId = requirement.ProjectId,
                TenantId = requirement.TenantId,
                CreatedAt = requirement.CreatedAt,
                UpdatedAt = requirement.ModifiedAt
            };
        }

        public async Task DeleteRequirementAsync(
            Guid requirementId,
            Guid projectId,
            string tenantId
        )
        {
            var requirement = await _context.Requirements.FirstOrDefaultAsync(r =>
                r.Id == requirementId
                && r.ProjectId == projectId
                && r.TenantId == tenantId
                && !r.IsDeleted
            );

            if (requirement == null)
                throw new Exception("Requirement not found.");

            requirement.IsDeleted = true;
            requirement.ModifiedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task LinkTestCaseToRequirementAsync(
            Guid requirementId,
            Guid projectId,
            string tenantId,
            Guid testCaseId
        )
        {
            var requirement = await _context.Requirements.FirstOrDefaultAsync(r =>
                r.Id == requirementId
                && r.ProjectId == projectId
                && r.TenantId == tenantId
                && !r.IsDeleted
            );

            if (requirement == null)
                throw new Exception("Requirement not found.");

            var testCase = await _context.TestCases.FirstOrDefaultAsync(tc =>
                tc.Id == testCaseId
                //&& tc.ProjectId == projectId
                && tc.TenantId == tenantId
                && !tc.IsDeleted
            );

            if (testCase == null)
                throw new Exception("Test case not found.");

            var existingLink = await _context.RequirementTestCases.FirstOrDefaultAsync(rtc =>
                rtc.RequirementId == requirementId && rtc.TestCaseId == testCaseId && !rtc.IsDeleted
            );

            if (existingLink != null)
                throw new Exception("Test case is already linked to this requirement.");

            var link = new RequirementTestCase
            {
                RequirementId = requirementId,
                TestCaseId = testCaseId,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };

            await _context.RequirementTestCases.AddAsync(link);
            await _context.SaveChangesAsync();
        }

        public async Task UnlinkTestCaseFromRequirementAsync(
            Guid requirementId,
            Guid projectId,
            string tenantId,
            Guid testCaseId
        )
        {
            var requirement = await _context.Requirements.FirstOrDefaultAsync(r =>
                r.Id == requirementId
                && r.ProjectId == projectId
                && r.TenantId == tenantId
                && !r.IsDeleted
            );

            if (requirement == null)
                throw new Exception("Requirement not found.");

            var link = await _context.RequirementTestCases.FirstOrDefaultAsync(rtc =>
                rtc.RequirementId == requirementId && rtc.TestCaseId == testCaseId && !rtc.IsDeleted
            );

            if (link == null)
                throw new Exception("Test case is not linked to this requirement.");

            link.IsDeleted = true;
            link.ModifiedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        private RequirementDto MapToDto(Requirement requirement, List<Requirement> allRequirements)
        {
            return new RequirementDto
            {
                Id = requirement.Id,
                Title = requirement.Title,
                Description = requirement.Description,
                Tags = requirement.Tags,
                ProjectId = requirement.ProjectId,
                TenantId = requirement.TenantId,
                ParentRequirementId = requirement.ParentRequirementId,
                TestCaseIds = requirement
                    .RequirementTestCases?.Select(rt => rt.TestCaseId)
                    .ToList(),
                CreatedAt = requirement.CreatedAt,
                UpdatedAt = requirement.ModifiedAt,
                Children = allRequirements
                    .Where(r => r.ParentRequirementId == requirement.Id)
                    .Select(r => MapToDto(r, allRequirements))
                    .ToList()
            };
        }
    }
}
