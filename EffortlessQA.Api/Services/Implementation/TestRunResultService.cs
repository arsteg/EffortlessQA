using System.Text.Json;
using EffortlessQA.Api.Services.Interface;
using EffortlessQA.Data;
using EffortlessQA.Data.Dtos;
using EffortlessQA.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EffortlessQA.Api.Services.Implementation
{
    public class TestRunResultService : ITestRunResultService
    {
        private readonly EffortlessQAContext _context;
        private readonly IConfiguration _configuration;

        public TestRunResultService(EffortlessQAContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<TestRunResultDto> CreateTestRunResultAsync(
            Guid testRunId,
            string tenantId,
            CreateTestRunResultDto dto
        )
        {
            var testRun = await _context.TestRuns.FirstOrDefaultAsync(tr =>
                tr.Id == testRunId && tr.TenantId == tenantId && !tr.IsDeleted
            );

            if (testRun == null)
                throw new Exception("Test run not found.");

            var testCase = await _context.TestCases.FirstOrDefaultAsync(tc =>
                tc.Id == dto.TestCaseId
                && tc.TenantId == tenantId
                && !tc.IsDeleted
                && tc.TestSuite.ProjectId == testRun.ProjectId
            );

            if (testCase == null)
                throw new Exception(
                    "Test case not found or not associated with the test run's project."
                );

            var existingResult = await _context.TestRunResults.FirstOrDefaultAsync(trr =>
                trr.TestRunId == testRunId && trr.TestCaseId == dto.TestCaseId
            );

            if (existingResult != null)
                throw new Exception(
                    "Test run result already exists for this test case in the test run."
                );

            var testRunResult = new TestRunResult
            {
                Id = Guid.NewGuid(),
                TestCaseId = dto.TestCaseId,
                TestRunId = testRunId,
                Status = dto.Status,
                Comments = dto.Comments,
                Attachments = dto.Attachments,
                TenantId = tenantId,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };

            await _context.TestRunResults.AddAsync(testRunResult);
            await _context.SaveChangesAsync();

            return new TestRunResultDto
            {
                Id = testRunResult.Id,
                TestCaseId = testRunResult.TestCaseId,
                TestRunId = testRunResult.TestRunId,
                Status = testRunResult.Status,
                Comments = testRunResult.Comments,
                Attachments = testRunResult.Attachments,
                TenantId = testRunResult.TenantId,
                CreatedAt = testRunResult.CreatedAt,
                UpdatedAt = testRunResult.ModifiedAt
            };
        }

        public async Task<PagedResult<TestRunResultDto>> GetTestRunResultsAsync(
            Guid testRunId,
            string tenantId,
            int page,
            int limit,
            TestExecutionStatus[]? statuses
        )
        {
            var query = _context.TestRunResults.Where(trr =>
                trr.TestRunId == testRunId && trr.TenantId == tenantId
            );

            if (statuses != null && statuses.Length > 0)
            {
                query = query.Where(trr => statuses.Contains(trr.Status));
            }

            query = query.OrderBy(trr => trr.CreatedAt);

            var totalCount = await query.CountAsync();
            var testRunResults = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(trr => new TestRunResultDto
                {
                    Id = trr.Id,
                    TestCaseId = trr.TestCaseId,
                    TestRunId = trr.TestRunId,
                    Status = trr.Status,
                    Comments = trr.Comments,
                    Attachments = trr.Attachments,
                    TenantId = trr.TenantId,
                    CreatedAt = trr.CreatedAt,
                    UpdatedAt = trr.ModifiedAt
                })
                .ToListAsync();

            return new PagedResult<TestRunResultDto>
            {
                Items = testRunResults,
                TotalCount = totalCount,
                Page = page,
                Limit = limit
            };
        }

        public async Task<TestRunResultDto> GetTestRunResultAsync(
            Guid resultId,
            Guid testRunId,
            string tenantId
        )
        {
            var testRunResult = await _context.TestRunResults.FirstOrDefaultAsync(trr =>
                trr.Id == resultId && trr.TestRunId == testRunId && trr.TenantId == tenantId
            );

            if (testRunResult == null)
                throw new Exception("Test run result not found.");

            return new TestRunResultDto
            {
                Id = testRunResult.Id,
                TestCaseId = testRunResult.TestCaseId,
                TestRunId = testRunResult.TestRunId,
                Status = testRunResult.Status,
                Comments = testRunResult.Comments,
                Attachments = testRunResult.Attachments,
                TenantId = testRunResult.TenantId,
                CreatedAt = testRunResult.CreatedAt,
                UpdatedAt = testRunResult.ModifiedAt
            };
        }

        public async Task<TestRunResultDto> UpdateTestRunResultAsync(
            Guid resultId,
            Guid testRunId,
            string tenantId,
            UpdateTestRunResultDto dto
        )
        {
            var testRunResult = await _context.TestRunResults.FirstOrDefaultAsync(trr =>
                trr.Id == resultId && trr.TestRunId == testRunId && trr.TenantId == tenantId
            );

            if (testRunResult == null)
                throw new Exception("Test run result not found.");

            testRunResult.Status = dto.Status ?? testRunResult.Status;
            testRunResult.Comments = dto.Comments ?? testRunResult.Comments;
            testRunResult.Attachments =
                dto.Attachments != null ? dto.Attachments : testRunResult.Attachments;
            testRunResult.ModifiedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new TestRunResultDto
            {
                Id = testRunResult.Id,
                TestCaseId = testRunResult.TestCaseId,
                TestRunId = testRunResult.TestRunId,
                Status = testRunResult.Status,
                Comments = testRunResult.Comments,
                Attachments = testRunResult.Attachments,
                TenantId = testRunResult.TenantId,
                CreatedAt = testRunResult.CreatedAt,
                UpdatedAt = testRunResult.ModifiedAt
            };
        }

        public async Task<IList<TestRunResultDto>> BulkUpdateTestRunResultsAsync(
            Guid testRunId,
            string tenantId,
            BulkUpdateTestRunResultDto dto
        )
        {
            var testRun = await _context.TestRuns.FirstOrDefaultAsync(tr =>
                tr.Id == testRunId && tr.TenantId == tenantId && !tr.IsDeleted
            );

            if (testRun == null)
                throw new Exception("Test run not found.");

            var testRunResults = await _context
                .TestRunResults.Where(trr =>
                    trr.TestRunId == testRunId
                    && dto.ResultUpdates.Select(ru => ru.ResultId).Contains(trr.Id)
                )
                .ToListAsync();

            var updatedDtos = new List<TestRunResultDto>();

            foreach (var update in dto.ResultUpdates)
            {
                var testRunResult = testRunResults.FirstOrDefault(trr => trr.Id == update.ResultId);
                if (testRunResult == null)
                    throw new Exception($"Test run result {update.ResultId} not found.");

                testRunResult.Status = update.Status ?? testRunResult.Status;
                testRunResult.Comments = update.Comments ?? testRunResult.Comments;
                testRunResult.Attachments =
                    update.Attachments != null ? update.Attachments : testRunResult.Attachments;
                testRunResult.ModifiedAt = DateTime.UtcNow;

                updatedDtos.Add(
                    new TestRunResultDto
                    {
                        Id = testRunResult.Id,
                        TestCaseId = testRunResult.TestCaseId,
                        TestRunId = testRunResult.TestRunId,
                        Status = testRunResult.Status,
                        Comments = testRunResult.Comments,
                        Attachments = testRunResult.Attachments,
                        TenantId = testRunResult.TenantId,
                        CreatedAt = testRunResult.CreatedAt,
                        UpdatedAt = testRunResult.ModifiedAt
                    }
                );
            }

            await _context.SaveChangesAsync();

            return updatedDtos;
        }
    }
}
