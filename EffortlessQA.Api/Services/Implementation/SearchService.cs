using EffortlessQA.Api.Services.Interface;
using EffortlessQA.Data;
using EffortlessQA.Data.Dtos;
using EffortlessQA.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EffortlessQA.Api.Services.Implementation
{
    public class SearchService : ISearchService
    {
        private readonly EffortlessQAContext _context;
        private readonly IConfiguration _configuration;

        public SearchService(EffortlessQAContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<PagedResult<SearchResultDto>> GlobalSearchAsync(
            string tenantId,
            string query,
            string[]? tags,
            int page,
            int limit
        )
        {
            var results = new List<SearchResultDto>();

            // Search Test Cases
            var testCaseQuery = _context
                .TestCases.Where(tc => tc.TenantId == tenantId && !tc.IsDeleted)
                .Where(tc =>
                    tc.Title.Contains(query)
                    || (tc.Tags != null && tc.Tags.Any(t => t.Contains(query)))
                );

            if (tags != null && tags.Length > 0)
            {
                testCaseQuery = testCaseQuery.Where(tc =>
                    tc.Tags != null && tc.Tags.Any(t => tags.Contains(t))
                );
            }

            var testCases = await testCaseQuery
                .Select(tc => new SearchResultDto
                {
                    Id = tc.Id,
                    Title = tc.Title,
                    EntityType = "TestCase",
                    ProjectId = tc.TestSuite.ProjectId,
                    Tags = tc.Tags
                })
                .ToListAsync();

            results.AddRange(testCases);

            // Search Test Suites
            var testSuiteQuery = _context
                .TestSuites.Where(ts => ts.TenantId == tenantId && !ts.IsDeleted)
                .Where(ts => ts.Name.Contains(query));

            if (tags != null && tags.Length > 0)
            {
                testSuiteQuery = testSuiteQuery.Where(ts =>
                    ts.TestCases.Any(tc => tc.Tags != null && tc.Tags.Any(t => tags.Contains(t)))
                );
            }

            var testSuites = await testSuiteQuery
                .Select(ts => new SearchResultDto
                {
                    Id = ts.Id,
                    Title = ts.Name,
                    EntityType = "TestSuite",
                    ProjectId = ts.ProjectId,
                    Tags = ts
                        .TestCases.SelectMany(tc => tc.Tags ?? Array.Empty<string>())
                        .Distinct()
                        .ToArray()
                })
                .ToListAsync();

            results.AddRange(testSuites);

            // Search Test Runs
            var testRunQuery = _context
                .TestRuns.Where(tr => tr.TenantId == tenantId && !tr.IsDeleted)
                .Where(tr => tr.Name.Contains(query));

            if (tags != null && tags.Length > 0)
            {
                testRunQuery = testRunQuery.Where(tr =>
                    tr.TestRunResults.Any(trr =>
                        trr.TestCase.Tags != null && trr.TestCase.Tags.Any(t => tags.Contains(t))
                    )
                );
            }

            var testRuns = await testRunQuery
                .Select(tr => new SearchResultDto
                {
                    Id = tr.Id,
                    Title = tr.Name,
                    EntityType = "TestRun",
                    ProjectId = tr.ProjectId,
                    Tags = tr
                        .TestRunResults.SelectMany(trr =>
                            trr.TestCase.Tags ?? Array.Empty<string>()
                        )
                        .Distinct()
                        .ToArray()
                })
                .ToListAsync();

            results.AddRange(testRuns);

            // Search Requirements
            var requirementQuery = _context
                .Requirements.Where(r => r.TenantId == tenantId && !r.IsDeleted)
                .Where(r =>
                    r.Title.Contains(query)
                    || (r.Tags != null && r.Tags.Any(t => t.Contains(query)))
                );

            if (tags != null && tags.Length > 0)
            {
                requirementQuery = requirementQuery.Where(r =>
                    r.Tags != null && r.Tags.Any(t => tags.Contains(t))
                );
            }

            var requirements = await requirementQuery
                .Select(r => new SearchResultDto
                {
                    Id = r.Id,
                    Title = r.Title,
                    EntityType = "Requirement",
                    ProjectId = r.ProjectId,
                    Tags = r.Tags
                })
                .ToListAsync();

            results.AddRange(requirements);

            // Search Defects
            var defectQuery = _context
                .Defects.Where(d => d.TenantId == tenantId && !d.IsDeleted)
                .Where(d => d.Title.Contains(query));

            if (tags != null && tags.Length > 0)
            {
                defectQuery = defectQuery.Where(d =>
                    d.TestCase != null
                    && d.TestCase.Tags != null
                    && d.TestCase.Tags.Any(t => tags.Contains(t))
                );
            }

            var defects = await defectQuery
                .Select(d => new SearchResultDto
                {
                    Id = d.Id,
                    Title = d.Title,
                    EntityType = "Defect",
                    ProjectId =
                        d.TestRunResult != null
                            ? d.TestRunResult.TestRun.ProjectId
                            : (d.TestCase != null ? d.TestCase.TestSuite.ProjectId : Guid.Empty),
                    Tags = d.TestCase != null ? d.TestCase.Tags : Array.Empty<string>()
                })
                .ToListAsync();

            results.AddRange(defects);

            // Paginate and sort results
            var totalCount = results.Count;
            var pagedResults = results
                .OrderBy(r => r.Title)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToList();

            return new PagedResult<SearchResultDto>
            {
                Items = pagedResults,
                TotalCount = totalCount,
                Page = page,
                Limit = limit
            };
        }

        public async Task<PagedResult<RequirementDto>> FilterRequirementsAsync(
            Guid projectId,
            string tenantId,
            string[]? tags,
            int page,
            int limit
        )
        {
            var query = _context.Requirements.Where(r =>
                r.ProjectId == projectId && r.TenantId == tenantId && !r.IsDeleted
            );

            if (tags != null && tags.Length > 0)
            {
                query = query.Where(r => r.Tags != null && r.Tags.Any(t => tags.Contains(t)));
            }

            query = query.OrderBy(r => r.Title);

            var totalCount = await query.CountAsync();
            var requirements = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(r => new RequirementDto
                {
                    Id = r.Id,
                    Title = r.Title,
                    Description = r.Description,
                    Tags = r.Tags,
                    ProjectId = r.ProjectId,
                    TenantId = r.TenantId,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.ModifiedAt
                })
                .ToListAsync();

            return new PagedResult<RequirementDto>
            {
                Items = requirements,
                TotalCount = totalCount,
                Page = page,
                Limit = limit
            };
        }

        public async Task<PagedResult<TestCaseDto>> FilterTestCasesAsync(
            Guid projectId,
            string tenantId,
            string[]? tags,
            PriorityLevel[]? priorities,
            TestExecutionStatus[]? statuses,
            int page,
            int limit
        )
        {
            var query = _context.TestCases.Where(tc =>
                tc.TestSuite.ProjectId == projectId && tc.TenantId == tenantId && !tc.IsDeleted
            );

            if (tags != null && tags.Length > 0)
            {
                query = query.Where(tc => tc.Tags != null && tc.Tags.Any(t => tags.Contains(t)));
            }

            if (priorities != null && priorities.Length > 0)
            {
                query = query.Where(tc => priorities.Contains(tc.Priority));
            }

            if (statuses != null && statuses.Length > 0)
            {
                query = query.Where(tc =>
                    tc.TestRunResults.Any(trr => statuses.Contains(trr.Status))
                );
            }

            query = query.OrderBy(tc => tc.Title);

            var totalCount = await query.CountAsync();
            var testCases = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(tc => new TestCaseDto
                {
                    Id = tc.Id,
                    Title = tc.Title,
                    Description = tc.Description,
                    Priority = tc.Priority,
                    Tags = tc.Tags,
                    TestSuiteId = tc.TestSuiteId,
                    TenantId = tc.TenantId,
                    CreatedAt = tc.CreatedAt,
                    UpdatedAt = tc.ModifiedAt
                })
                .ToListAsync();

            return new PagedResult<TestCaseDto>
            {
                Items = testCases,
                TotalCount = totalCount,
                Page = page,
                Limit = limit
            };
        }

        public async Task<PagedResult<TestRunDto>> FilterTestRunsAsync(
            Guid projectId,
            string tenantId,
            TestExecutionStatus[]? statuses,
            Guid[]? assignedTesterIds,
            int page,
            int limit
        )
        {
            var query = _context.TestRuns.Where(tr =>
                tr.ProjectId == projectId && tr.TenantId == tenantId && !tr.IsDeleted
            );

            if (statuses != null && statuses.Length > 0)
            {
                query = query.Where(tr =>
                    tr.TestRunResults.Any(trr => statuses.Contains(trr.Status))
                );
            }

            if (assignedTesterIds != null && assignedTesterIds.Length > 0)
            {
                query = query.Where(tr =>
                    tr.AssignedTesterId.HasValue
                    && assignedTesterIds.Contains(tr.AssignedTesterId.Value)
                );
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

        public async Task<PagedResult<DefectDto>> FilterDefectsAsync(
            Guid projectId,
            string tenantId,
            SeverityLevel[]? severities,
            DefectStatus[]? statuses,
            int page,
            int limit
        )
        {
            var query = _context.Defects.Where(d =>
                (d.TestRunResult != null && d.TestRunResult.TestRun.ProjectId == projectId)
                || (d.TestCase != null && d.TestCase.TestSuite.ProjectId == projectId)
                    && d.TenantId == tenantId
                    && !d.IsDeleted
            );

            if (severities != null && severities.Length > 0)
            {
                query = query.Where(d => severities.Contains(d.Severity));
            }

            if (statuses != null && statuses.Length > 0)
            {
                query = query.Where(d => statuses.Contains(d.Status));
            }

            query = query.OrderBy(d => d.Title);

            var totalCount = await query.CountAsync();
            var defects = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(d => new DefectDto
                {
                    Id = d.Id,
                    Title = d.Title,
                    Description = d.Description,
                    Severity = d.Severity,
                    Status = d.Status,
                    TestRunResultId = d.TestRunResultId,
                    TestCaseId = d.TestCaseId,
                    TenantId = d.TenantId,
                    AssignedUserId = d.AssignedUserId,
                    ResolutionNotes = d.ResolutionNotes,
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.ModifiedAt
                })
                .ToListAsync();

            return new PagedResult<DefectDto>
            {
                Items = defects,
                TotalCount = totalCount,
                Page = page,
                Limit = limit
            };
        }
    }
}
