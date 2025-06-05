using EffortlessQA.Api.Services.Interface;
using EffortlessQA.Data;
using EffortlessQA.Data.Dtos;
using EffortlessQA.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EffortlessQA.Api.Services.Implementation
{
    public class TestSuiteService : ITestSuiteService
    {
        private readonly EffortlessQAContext _context;
        private readonly IConfiguration _configuration;

        public TestSuiteService(EffortlessQAContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<TestSuiteDto> CreateTestSuiteAsync(
            Guid projectId,
            string tenantId,
            CreateTestSuiteDto dto
        )
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p =>
                p.Id == projectId && p.TenantId == tenantId && !p.IsDeleted
            );

            if (project == null)
                throw new Exception("Project not found.");

            if (dto.ParentSuiteId.HasValue)
            {
                var parentSuite = await _context.TestSuites.FirstOrDefaultAsync(ts =>
                    ts.Id == dto.ParentSuiteId
                    && ts.ProjectId == projectId
                    && ts.TenantId == tenantId
                    && !ts.IsDeleted
                );
                if (parentSuite == null)
                    throw new Exception("Parent test suite not found.");
            }

            var testSuite = new TestSuite
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                ProjectId = projectId,
                TenantId = tenantId,
                ParentSuiteId = dto.ParentSuiteId,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };

            await _context.TestSuites.AddAsync(testSuite);
            await _context.SaveChangesAsync();

            return new TestSuiteDto
            {
                Id = testSuite.Id,
                Name = testSuite.Name,
                Description = testSuite.Description,
                ProjectId = testSuite.ProjectId,
                TenantId = testSuite.TenantId,
                ParentSuiteId = testSuite.ParentSuiteId
            };
        }

        public async Task<PagedResult<TestSuiteDto>> GetTestSuitesAsync(
            Guid projectId,
            string tenantId,
            int page,
            int limit,
            string? filter
        )
        {
            var query = _context.TestSuites.Where(ts =>
                ts.ProjectId == projectId && ts.TenantId == tenantId && !ts.IsDeleted
            );

            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(ts => ts.Name.Contains(filter));
            }

            query = query.OrderBy(ts => ts.Name);

            var totalCount = await query.CountAsync();
            var testSuites = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(ts => new TestSuiteDto
                {
                    Id = ts.Id,
                    Name = ts.Name,
                    Description = ts.Description,
                    ProjectId = ts.ProjectId,
                    TenantId = ts.TenantId,
                    ParentSuiteId = ts.ParentSuiteId
                })
                .ToListAsync();

            return new PagedResult<TestSuiteDto>
            {
                Items = testSuites,
                TotalCount = totalCount,
                Page = page,
                Limit = limit
            };
        }

		public async Task<PagedResult<TestSuiteDto>> GetTestSuitesAsync(
			string tenantId,
			int page,
			int limit,
			string? filter
		)
		{
			var query = _context.TestSuites.Where(ts => ts.TenantId == tenantId && !ts.IsDeleted);

			// Parse the filter parameter
			string? nameFilter = null;
			string? sortField = null;
			bool sortAscending = true;

			if (!string.IsNullOrEmpty(filter))
			{
				// Split filter by commas to handle multiple conditions
				var filterConditions = filter
					.Split(',',StringSplitOptions.RemoveEmptyEntries)
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
				query = query.Where(ts => ts.Name.ToLower().Contains(nameFilter.ToLower()));
			}

			// Apply sorting
			if (sortField == "name")
			{
				query = sortAscending
					? query.OrderBy(ts => ts.Name)
					: query.OrderByDescending(ts => ts.Name);
			}
			else if (sortField == "description")
			{
				query = sortAscending
					? query.OrderBy(ts => ts.Description)
					: query.OrderByDescending(ts => ts.Description);
			}
			else
			{
				// Default sorting
				query = query.OrderBy(ts => ts.Name);
			}

			var totalCount = await query.CountAsync();
			var testSuites = await query
				.Skip((page - 1) * limit)
				.Take(limit)
				.Select(ts => new TestSuiteDto
				{
					Id = ts.Id,
					Name = ts.Name,
					Description = ts.Description,
					ProjectId = ts.ProjectId,
					TenantId = ts.TenantId,
					ParentSuiteId = ts.ParentSuiteId
				})
				.ToListAsync();

			return new PagedResult<TestSuiteDto>
			{
				Items = testSuites,
				TotalCount = totalCount,
				Page = page,
				Limit = limit
			};
		}
		public async Task<TestSuiteDto> GetTestSuiteAsync(
            Guid testSuiteId,
            Guid projectId,
            string tenantId
        )
        {
            var testSuite = await _context.TestSuites.FirstOrDefaultAsync(ts =>
                ts.Id == testSuiteId
                && ts.ProjectId == projectId
                && ts.TenantId == tenantId
                && !ts.IsDeleted
            );

            if (testSuite == null)
                throw new Exception("Test suite not found.");

            return new TestSuiteDto
            {
                Id = testSuite.Id,
                Name = testSuite.Name,
                Description = testSuite.Description,
                ProjectId = testSuite.ProjectId,
                TenantId = testSuite.TenantId,
                ParentSuiteId = testSuite.ParentSuiteId
            };
        }

        public async Task<TestSuiteDto> UpdateTestSuiteAsync(
            Guid testSuiteId,
            Guid projectId,
            string tenantId,
            UpdateTestSuiteDto dto
        )
        {
            var testSuite = await _context.TestSuites.FirstOrDefaultAsync(ts =>
                ts.Id == testSuiteId
                && ts.ProjectId == projectId
                && ts.TenantId == tenantId
                && !ts.IsDeleted
            );

            if (testSuite == null)
                throw new Exception("Test suite not found.");

            if (dto.ParentSuiteId.HasValue)
            {
                var parentSuite = await _context.TestSuites.FirstOrDefaultAsync(ts =>
                    ts.Id == dto.ParentSuiteId
                    && ts.ProjectId == projectId
                    && ts.TenantId == tenantId
                    && !ts.IsDeleted
                );
                if (parentSuite == null)
                    throw new Exception("Parent test suite not found.");
            }

            testSuite.Name = dto.Name ?? testSuite.Name;
            testSuite.Description = dto.Description ?? testSuite.Description;
            testSuite.ParentSuiteId = dto.ParentSuiteId ?? testSuite.ParentSuiteId;
            testSuite.ModifiedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new TestSuiteDto
            {
                Id = testSuite.Id,
                Name = testSuite.Name,
                Description = testSuite.Description,
                ProjectId = testSuite.ProjectId,
                TenantId = testSuite.TenantId,
                ParentSuiteId = testSuite.ParentSuiteId
            };
        }

        public async Task DeleteTestSuiteAsync(Guid testSuiteId, Guid projectId, string tenantId)
        {
            var testSuite = await _context.TestSuites.FirstOrDefaultAsync(ts =>
                ts.Id == testSuiteId
                && ts.ProjectId == projectId
                && ts.TenantId == tenantId
                && !ts.IsDeleted
            );

            if (testSuite == null)
                throw new Exception("Test suite not found.");

            testSuite.IsDeleted = true;
            testSuite.ModifiedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
