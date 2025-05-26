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

        public TestRunService(EffortlessQAContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<TestRunDto>> GetTestRunsAsync(
            Guid projectId,
            string tenantId,
            int page,
            int limit,
            string sort,
            string filter
        )
        {
            var query = _context
                .TestRuns.AsNoTracking()
                .Where(tr => tr.ProjectId == projectId && tr.TenantId == tenantId && !tr.IsDeleted);

            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(tr =>
                    tr.Name.Contains(filter) || tr.Description.Contains(filter)
                );
            }

            //query = query.OrderBy(sort);
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(tr => new TestRunDto
                {
                    Id = tr.Id,
                    Name = tr.Name,
                    Description = tr.Description,
                    AssignedTesterId = tr.AssignedTesterId,
                    ProjectId = tr.ProjectId,
                    TenantId = tr.TenantId
                })
                .ToListAsync();

            return new PagedResult<TestRunDto> { Items = items, TotalCount = totalCount };
        }

        public async Task<TestRunDto> CreateTestRunAsync(
            Guid projectId,
            TestRunCreateDto dto,
            string tenantId
        )
        {
            var project = await _context.Projects.FindAsync(projectId);
            if (project == null || project.TenantId != tenantId)
                throw new Exception("Project not found or access denied.");

            var testRun = new TestRun
            {
                Name = dto.Name,
                Description = dto.Description,
                AssignedTesterId = dto.AssignedTesterId,
                ProjectId = projectId,
                TenantId = tenantId
            };

            _context.TestRuns.Add(testRun);
            await _context.SaveChangesAsync();

            foreach (var testCaseId in dto.TestCaseIds)
            {
                var testCase = await _context.TestCases.FindAsync(testCaseId);
                if (testCase != null && testCase.TenantId == tenantId)
                {
                    _context.TestRunResults.Add(
                        new TestRunResult
                        {
                            TestCaseId = testCaseId,
                            TestRunId = testRun.Id,
                            Status = TestExecutionStatus.Skipped,
                            TenantId = tenantId
                        }
                    );
                }
            }

            await _context.SaveChangesAsync();

            return new TestRunDto
            {
                Id = testRun.Id,
                Name = testRun.Name,
                Description = testRun.Description,
                AssignedTesterId = testRun.AssignedTesterId,
                ProjectId = testRun.ProjectId,
                TenantId = testRun.TenantId
            };
        }
    }
}
