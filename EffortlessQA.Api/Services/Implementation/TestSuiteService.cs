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

        public TestSuiteService(EffortlessQAContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<TestSuiteDto>> GetTestSuitesAsync(
            Guid projectId,
            string tenantId,
            int page,
            int limit,
            string sort,
            string filter
        )
        {
            var query = _context
                .TestSuites.AsNoTracking()
                .Where(ts => ts.ProjectId == projectId && ts.TenantId == tenantId && !ts.IsDeleted);

            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(ts =>
                    ts.Name.Contains(filter) || ts.Description.Contains(filter)
                );
            }

            //query = query.OrderBy(sort);
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(ts => new TestSuiteDto
                {
                    Id = ts.Id,
                    Name = ts.Name,
                    Description = ts.Description,
                    ProjectId = ts.ProjectId,
                    TenantId = ts.TenantId
                })
                .ToListAsync();

            return new PagedResult<TestSuiteDto> { Items = items, TotalCount = totalCount };
        }

        public async Task<TestSuiteDto> CreateTestSuiteAsync(
            Guid projectId,
            TestSuiteCreateDto dto,
            string tenantId
        )
        {
            var project = await _context.Projects.FindAsync(projectId);
            if (project == null || project.TenantId != tenantId)
                throw new Exception("Project not found or access denied.");

            var testSuite = new TestSuite
            {
                Name = dto.Name,
                Description = dto.Description,
                ProjectId = projectId,
                TenantId = tenantId
            };

            _context.TestSuites.Add(testSuite);
            await _context.SaveChangesAsync();

            return new TestSuiteDto
            {
                Id = testSuite.Id,
                Name = testSuite.Name,
                Description = testSuite.Description,
                ProjectId = testSuite.ProjectId,
                TenantId = testSuite.TenantId
            };
        }

        public async Task<PagedResult<TestCaseDto>> GetTestCasesAsync(
            Guid testSuiteId,
            string tenantId,
            int page,
            int limit,
            string sort,
            string filter
        )
        {
            var query = _context
                .TestCases.AsNoTracking()
                .Where(tc =>
                    tc.TestSuiteId == testSuiteId && tc.TenantId == tenantId && !tc.IsDeleted
                );

            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(tc =>
                    tc.Title.Contains(filter) || tc.Description.Contains(filter)
                );
            }

            //query = query.OrderBy(sort);
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(tc => new TestCaseDto
                {
                    Id = tc.Id,
                    Title = tc.Title,
                    Description = tc.Description,
                    Steps = tc.Steps,
                    ExpectedResults = tc.ExpectedResults,
                    Priority = tc.Priority,
                    Tags = tc.Tags,
                    TestSuiteId = tc.TestSuiteId,
                    TenantId = tc.TenantId
                })
                .ToListAsync();

            return new PagedResult<TestCaseDto> { Items = items, TotalCount = totalCount };
        }

        public async Task<TestCaseDto> CreateTestCaseAsync(
            Guid testSuiteId,
            TestCaseCreateDto dto,
            string tenantId
        )
        {
            var testSuite = await _context.TestSuites.FindAsync(testSuiteId);
            if (testSuite == null || testSuite.TenantId != tenantId)
                throw new Exception("TestSuite not found or access denied.");

            var testCase = new TestCase
            {
                Title = dto.Title,
                Description = dto.Description,
                Steps = dto.Steps,
                ExpectedResults = dto.ExpectedResults,
                Priority = dto.Priority,
                Tags = dto.Tags,
                TestSuiteId = testSuiteId,
                TenantId = tenantId
            };

            _context.TestCases.Add(testCase);
            await _context.SaveChangesAsync();

            return new TestCaseDto
            {
                Id = testCase.Id,
                Title = testCase.Title,
                Description = testCase.Description,
                Steps = testCase.Steps,
                ExpectedResults = testCase.ExpectedResults,
                Priority = testCase.Priority,
                Tags = testCase.Tags,
                TestSuiteId = testCase.TestSuiteId,
                TenantId = testCase.TenantId
            };
        }
    }
}
