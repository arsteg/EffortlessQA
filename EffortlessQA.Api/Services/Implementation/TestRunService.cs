using System;
using System.Linq;
using EffortlessQA.Api.Services.Interface;
using EffortlessQA.Data;
using EffortlessQA.Data.Dtos;
using EffortlessQA.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EffortlessQA.Api.Services.Implementation
{
    public class TestRunService : ITestRunService
    {
        private readonly EffortlessQAContext _context;
        private readonly IConfiguration _configuration;

        public TestRunService(EffortlessQAContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<TestRunDto> CreateTestRunAsync(
            Guid projectId,
            string tenantId,
            CreateTestRunDto dto
        )
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p =>
                p.Id == projectId && p.TenantId == tenantId && !p.IsDeleted
            );

            if (project == null)
                throw new Exception("Project not found.");

            if (dto.AssignedTesterId.HasValue)
            {
                var tester = await _context.Users.FirstOrDefaultAsync(u =>
                    u.Id == dto.AssignedTesterId && u.TenantId == tenantId && !u.IsDeleted
                );
                if (tester == null)
                    throw new Exception("Assigned tester not found.");
            }

            var testRun = new TestRun
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                AssignedTesterId = dto.AssignedTesterId,
                ProjectId = projectId,
                TenantId = tenantId,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };

            await _context.TestRuns.AddAsync(testRun);
            await _context.SaveChangesAsync();

            return new TestRunDto
            {
                Id = testRun.Id,
                Name = testRun.Name,
                Description = testRun.Description,
                AssignedTesterId = testRun.AssignedTesterId,
                ProjectId = testRun.ProjectId,
                TenantId = testRun.TenantId,
                CreatedAt = testRun.CreatedAt,
                UpdatedAt = testRun.ModifiedAt
            };
        }

        public async Task<PagedResult<TestRunDto>> GetTestRunsAsync(
            Guid projectId,
            string tenantId,
            int page,
            int limit,
            string? filter,
            string[]? statuses
        )
        {
            var query = _context.TestRuns.Where(tr =>
                tr.ProjectId == projectId && tr.TenantId == tenantId && !tr.IsDeleted
            );

            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(tr => tr.Name.Contains(filter));
            }

            // Assuming status is derived from TestRunResults (e.g., Pass, Fail, Pending)
            if (statuses != null && statuses.Length > 0)
            {
                //query = query.Where(tr =>
                //    tr.TestRunResults.Any(trr => statuses.Contains(trr.Status))
                //);
            }

            query = query.OrderBy(tr => tr.Name);

            var totalCount = await query.CountAsync();
            var testRuns = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(tr => new TestRunDto
                {
                    Id = tr.Id,
                    Name = tr.Name,
                    Description = tr.Description,
                    AssignedTesterId = tr.AssignedTesterId,
                    ProjectId = tr.ProjectId,
                    TenantId = tr.TenantId,
                    CreatedAt = tr.CreatedAt,
                    UpdatedAt = tr.ModifiedAt
                })
                .ToListAsync();

            return new PagedResult<TestRunDto>
            {
                Items = testRuns,
                TotalCount = totalCount,
                Page = page,
                Limit = limit
            };
        }

		public async Task<PagedResult<TestRunDto>> GetAllTestRunsAsync(
	            string tenantId,
	            int page,
	            int limit,
	            string? filter,
	            string[]? statuses
            )
		{
			var query = _context.TestRuns.Where(tr => tr.TenantId == tenantId && !tr.IsDeleted);

			string? nameFilter = null;
			string? sortField = null;
			bool sortAscending = true;

			if (!string.IsNullOrEmpty(filter))
			{
				var filterConditions = filter
					.Split(',',StringSplitOptions.RemoveEmptyEntries)
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
					else if (filterParts.Length == 2 && filterParts[0].ToLower() == "name")
					{
						nameFilter = filterParts[1];
					}
					else
					{
						nameFilter = condition;
					}
				}
			}

			if (!string.IsNullOrEmpty(nameFilter))
			{
				query = query.Where(tr => tr.Name.ToLower().Contains(nameFilter.ToLower()));
			}

			if (sortField == "name")
			{
				query = sortAscending
					? query.OrderBy(tr => tr.Name)
					: query.OrderByDescending(tr => tr.Name);
			}
			else if (sortField == "description")
			{
				query = sortAscending
					? query.OrderBy(tr => tr.Description)
					: query.OrderByDescending(tr => tr.Description);
			}
			else if (sortField == "createdat")
			{
				query = sortAscending
					? query.OrderBy(tr => tr.CreatedAt)
					: query.OrderByDescending(tr => tr.CreatedAt);
			}
			else if (sortField == "updatedat")
			{
				query = sortAscending
					? query.OrderBy(tr => tr.ModifiedAt)
					: query.OrderByDescending(tr => tr.ModifiedAt);
			}
			else
			{
				query = query.OrderBy(tr => tr.Name);
			}

			var totalCount = await query.CountAsync();
			var testRuns = await query
				.Skip((page - 1) * limit)
				.Take(limit)
				.Select(tr => new TestRunDto
				{
					Id = tr.Id,
					Name = tr.Name,
					Description = tr.Description,
					AssignedTesterId = tr.AssignedTesterId,
					ProjectId = tr.ProjectId,
					TenantId = tr.TenantId,
					CreatedAt = tr.CreatedAt,
					UpdatedAt = tr.ModifiedAt
				})
				.ToListAsync();

			return new PagedResult<TestRunDto>
			{
				Items = testRuns,
				TotalCount = totalCount,
				Page = page,
				Limit = limit
			};
		}

		public async Task<TestRunDto> GetTestRunAsync(
            Guid testRunId,
            Guid projectId,
            string tenantId
        )
        {
            var testRun = await _context.TestRuns.FirstOrDefaultAsync(tr =>
                tr.Id == testRunId
                && tr.ProjectId == projectId
                && tr.TenantId == tenantId
                && !tr.IsDeleted
            );

            if (testRun == null)
                throw new Exception("Test run not found.");

            return new TestRunDto
            {
                Id = testRun.Id,
                Name = testRun.Name,
                Description = testRun.Description,
                AssignedTesterId = testRun.AssignedTesterId,
                ProjectId = testRun.ProjectId,
                TenantId = testRun.TenantId,
                CreatedAt = testRun.CreatedAt,
                UpdatedAt = testRun.ModifiedAt
            };
        }

        public async Task<TestRunDto> UpdateTestRunAsync(
            Guid testRunId,
            Guid projectId,
            string tenantId,
            UpdateTestRunDto dto
        )
        {
            var testRun = await _context.TestRuns.FirstOrDefaultAsync(tr =>
                tr.Id == testRunId
                && tr.ProjectId == projectId
                && tr.TenantId == tenantId
                && !tr.IsDeleted
            );

            if (testRun == null)
                throw new Exception("Test run not found.");

            if (dto.AssignedTesterId.HasValue)
            {
                var tester = await _context.Users.FirstOrDefaultAsync(u =>
                    u.Id == dto.AssignedTesterId && u.TenantId == tenantId && !u.IsDeleted
                );
                if (tester == null)
                    throw new Exception("Assigned tester not found.");
            }

            testRun.Name = dto.Name ?? testRun.Name;
            testRun.Description = dto.Description ?? testRun.Description;
            testRun.AssignedTesterId = dto.AssignedTesterId ?? testRun.AssignedTesterId;
            testRun.ModifiedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new TestRunDto
            {
                Id = testRun.Id,
                Name = testRun.Name,
                Description = testRun.Description,
                AssignedTesterId = testRun.AssignedTesterId,
                ProjectId = testRun.ProjectId,
                TenantId = testRun.TenantId,
                CreatedAt = testRun.CreatedAt,
                UpdatedAt = testRun.ModifiedAt
            };
        }

        public async Task DeleteTestRunAsync(Guid testRunId, Guid projectId, string tenantId)
        {
            var testRun = await _context.TestRuns.FirstOrDefaultAsync(tr =>
                tr.Id == testRunId
                && tr.ProjectId == projectId
                && tr.TenantId == tenantId
                && !tr.IsDeleted
            );

            if (testRun == null)
                throw new Exception("Test run not found.");

            testRun.IsDeleted = true;
            testRun.ModifiedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
