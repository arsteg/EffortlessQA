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

		public TestSuiteService( EffortlessQAContext context,IConfiguration configuration )
		{
			_context = context;
			_configuration = configuration;
		}

		public async Task<TestSuiteDto> CreateTestSuiteAsync(
			Guid projectId,
			string tenantId,
			CreateTestSuiteDto dto )
		{
			var project = await _context.Projects.FirstOrDefaultAsync(p =>
				p.Id == projectId && p.TenantId == tenantId && !p.IsDeleted);

			if (project == null)
				throw new Exception("Project not found.");

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

			return MapToDto(testSuite,new List<TestSuite>());
		}

		public async Task<PagedResult<TestSuiteDto>> GetTestSuitesAsync(
			Guid projectId,
			string tenantId,
			int page,
			int limit,
			string? filter )
		{
			var query = _context.TestSuites.Where(ts =>
				ts.ProjectId == projectId
				&& ts.TenantId == tenantId
				&& !ts.IsDeleted
				&& ts.ParentSuiteId == null);

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
				query = query.Where(ts => ts.Name.ToLower().Contains(nameFilter.ToLower()));
			}

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
			else if (sortField == "createdat")
			{
				query = sortAscending
					? query.OrderBy(ts => ts.CreatedAt)
					: query.OrderByDescending(ts => ts.CreatedAt);
			}
			else if (sortField == "updatedat")
			{
				query = sortAscending
					? query.OrderBy(ts => ts.ModifiedAt)
					: query.OrderByDescending(ts => ts.ModifiedAt);
			}
			else
			{
				query = query.OrderBy(ts => ts.Name);
			}

			var totalCount = await query.CountAsync();
			var topLevelTestSuites = await query
				.Skip((page - 1) * limit)
				.Take(limit)
				.ToListAsync();

			var allTestSuites = await _context.TestSuites
				.Where(ts => ts.ProjectId == projectId && ts.TenantId == tenantId && !ts.IsDeleted)
				.ToListAsync();

			var testSuiteDtos = topLevelTestSuites
				.Select(ts => MapToDto(ts,allTestSuites))
				.ToList();

			return new PagedResult<TestSuiteDto>
			{
				Items = testSuiteDtos,
				TotalCount = totalCount,
				Page = page,
				Limit = limit
			};
		}

		public async Task<PagedResult<TestSuiteDto>> GetTestSuitesAsync(
			string tenantId,
			int page,
			int limit,
			string? filter )
		{
			var query = _context.TestSuites.Where(ts =>
				ts.TenantId == tenantId && !ts.IsDeleted && ts.ParentSuiteId == null);

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
				query = query.Where(ts => ts.Name.ToLower().Contains(nameFilter.ToLower()));
			}

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
			else if (sortField == "createdat")
			{
				query = sortAscending
					? query.OrderBy(ts => ts.CreatedAt)
					: query.OrderByDescending(ts => ts.CreatedAt);
			}
			else if (sortField == "updatedat")
			{
				query = sortAscending
					? query.OrderBy(ts => ts.ModifiedAt)
					: query.OrderByDescending(ts => ts.ModifiedAt);
			}
			else
			{
				query = query.OrderBy(ts => ts.Name);
			}

			var totalCount = await query.CountAsync();
			var topLevelTestSuites = await query
				.Skip((page - 1) * limit)
				.Take(limit)
				.ToListAsync();

			var allTestSuites = await _context.TestSuites
				.Where(ts => ts.TenantId == tenantId && !ts.IsDeleted)
				.ToListAsync();

			var testSuiteDtos = topLevelTestSuites
				.Select(ts => MapToDto(ts,allTestSuites))
				.ToList();

			return new PagedResult<TestSuiteDto>
			{
				Items = testSuiteDtos,
				TotalCount = totalCount,
				Page = page,
				Limit = limit
			};
		}

		public async Task<TestSuiteDto> GetTestSuiteAsync(
			Guid testSuiteId,
			Guid projectId,
			string tenantId )
		{
			var testSuite = await _context.TestSuites.FirstOrDefaultAsync(ts =>
				ts.Id == testSuiteId
				&& ts.ProjectId == projectId
				&& ts.TenantId == tenantId
				&& !ts.IsDeleted);

			if (testSuite == null)
				throw new Exception("Test suite not found.");

			var allTestSuites = await _context.TestSuites
				.Where(ts => ts.ProjectId == projectId && ts.TenantId == tenantId && !ts.IsDeleted)
				.ToListAsync();

			return MapToDto(testSuite,allTestSuites);
		}

		public async Task<TestSuiteDto> UpdateTestSuiteAsync(
			Guid testSuiteId,
			Guid projectId,
			string tenantId,
			UpdateTestSuiteDto dto )
		{
			var testSuite = await _context.TestSuites.FirstOrDefaultAsync(ts =>
				ts.Id == testSuiteId
				&& ts.ProjectId == projectId
				&& ts.TenantId == tenantId
				&& !ts.IsDeleted);

			if (testSuite == null)
				throw new Exception("Test suite not found.");

			if (dto.ParentSuiteId.HasValue)
			{
				var parentSuite = await _context.TestSuites.FirstOrDefaultAsync(ts =>
					ts.Id == dto.ParentSuiteId
					&& ts.ProjectId == projectId
					&& ts.TenantId == tenantId
					&& !ts.IsDeleted);
				if (parentSuite == null)
					throw new Exception("Parent test suite not found.");
			}

			testSuite.Name = dto.Name ?? testSuite.Name;
			testSuite.Description = dto.Description ?? testSuite.Description;
			testSuite.ParentSuiteId = dto.ParentSuiteId ?? testSuite.ParentSuiteId;
			testSuite.ModifiedAt = DateTime.UtcNow;

			await _context.SaveChangesAsync();

			var allTestSuites = await _context.TestSuites
				.Where(ts => ts.ProjectId == projectId && ts.TenantId == tenantId && !ts.IsDeleted)
				.ToListAsync();

			return MapToDto(testSuite,allTestSuites);
		}

		public async Task DeleteTestSuiteAsync(
			Guid testSuiteId,
			Guid projectId,
			string tenantId )
		{
			var testSuite = await _context.TestSuites.FirstOrDefaultAsync(ts =>
				ts.Id == testSuiteId
				&& ts.ProjectId == projectId
				&& ts.TenantId == tenantId
				&& !ts.IsDeleted);

			if (testSuite == null)
				throw new Exception("Test suite not found.");

			testSuite.IsDeleted = true;
			testSuite.ModifiedAt = DateTime.UtcNow;
			await _context.SaveChangesAsync();
		}

		public async Task LinkTestSuiteToRequirementAsync(
			Guid requirementId,
			Guid projectId,
			string tenantId,
			Guid testSuiteId
		)
		{
			var requirement = await _context.Requirements.FirstOrDefaultAsync(r =>
				r.Id == requirementId
				&& r.TenantId == tenantId
				&& !r.IsDeleted
			);

			if (requirement == null)
				throw new Exception("Requirement not found.");

			var testSuite = await _context.TestSuites.FirstOrDefaultAsync(ts =>
				ts.Id == testSuiteId
				&& ts.TenantId == tenantId
				&& !ts.IsDeleted
			);

			if (testSuite == null)
				throw new Exception("Test suite not found.");

			var existingLink = await _context.RequirementTestSuites.FirstOrDefaultAsync(rts =>
				rts.RequirementId == requirementId && rts.TestSuiteId == testSuiteId && !rts.IsDeleted
			);

			if (existingLink != null)
				throw new Exception("Test suite is already linked to this requirement.");

			var link = new RequirementTestSuite
			{
				RequirementId = requirementId,
				TestSuiteId = testSuiteId,
				CreatedAt = DateTime.UtcNow,
				ModifiedAt = DateTime.UtcNow
			};

			await _context.RequirementTestSuites.AddAsync(link);
			await _context.SaveChangesAsync();
		}

		public async Task UnlinkTestSuiteFromRequirementAsync(
			Guid requirementId,
			Guid projectId,
			string tenantId,
			Guid testSuiteId
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

			var link = await _context.RequirementTestSuites.FirstOrDefaultAsync(rts =>
				rts.RequirementId == requirementId && rts.TestSuiteId == testSuiteId && !rts.IsDeleted
			);

			if (link == null)
				throw new Exception("Test suite is not linked to this requirement.");

			link.IsDeleted = true;
			link.ModifiedAt = DateTime.UtcNow;
			await _context.SaveChangesAsync();
		}

		private TestSuiteDto MapToDto( TestSuite testSuite,List<TestSuite> allTestSuites )
		{
			return new TestSuiteDto
			{
				Id = testSuite.Id,
				Name = testSuite.Name,
				Description = testSuite.Description,
				ProjectId = testSuite.ProjectId,
				TenantId = testSuite.TenantId,
				ParentSuiteId = testSuite.ParentSuiteId,
				CreatedAt = testSuite.CreatedAt,
				ModifiedAt = testSuite.ModifiedAt,
				Children = allTestSuites
					.Where(ts => ts.ParentSuiteId == testSuite.Id && !ts.IsDeleted)
					.Select(ts => MapToDto(ts,allTestSuites))
					.ToList()
			};
		}
	}
}