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

        public TestRunResultService(EffortlessQAContext context)
        {
            _context = context;
        }

        public async Task<TestRunResultDto> CreateTestRunResultAsync(
            Guid testRunId,
            TestRunResultCreateDto dto,
            string tenantId
        )
        {
            var testRun = await _context.TestRuns.FindAsync(testRunId);
            if (testRun == null || testRun.TenantId != tenantId)
                throw new Exception("TestRun not found or access denied.");

            var testCase = await _context.TestCases.FindAsync(dto.TestCaseId);
            if (testCase == null || testCase.TenantId != tenantId)
                throw new Exception("TestCase not found or access denied.");

            var testRunResult = new TestRunResult
            {
                TestCaseId = dto.TestCaseId,
                TestRunId = testRunId,
                Status = dto.Status,
                Comments = dto.Comments,
                Attachments = dto.Attachments,
                TenantId = tenantId
            };

            _context.TestRunResults.Add(testRunResult);
            await _context.SaveChangesAsync();

            return new TestRunResultDto
            {
                Id = testRunResult.Id,
                TestCaseId = testRunResult.TestCaseId,
                TestRunId = testRunResult.TestRunId,
                Status = testRunResult.Status,
                Comments = testRunResult.Comments,
                Attachments = testRunResult.Attachments,
                TenantId = testRunResult.TenantId
            };
        }

        public async Task BulkUpdateTestRunResultsAsync(
            Guid testRunId,
            TestRunResultBulkUpdateDto dto,
            string tenantId
        )
        {
            var testRun = await _context.TestRuns.FindAsync(testRunId);
            if (testRun == null || testRun.TenantId != tenantId)
                throw new Exception("TestRun not found or access denied.");

            //foreach (var resultDto in dto.Results)
            //{
            //    var testRunResult = await _context.TestRunResults.FirstOrDefaultAsync(trr =>
            //        trr.TestRunId == testRunId
            //        && trr.TestCaseId == resultDto.TestCaseId
            //        && trr.TenantId == tenantId
            //    );

            //    if (testRunResult != null)
            //    {
            //        testRunResult.Status = resultDto.Status;
            //        testRunResult.Comments = resultDto.Comments;
            //        testRunResult.Attachments = resultDto.Attachments;
            //        testRunResult.ModifiedAt = DateTime.UtcNow;
            //    }
            //    else
            //    {
            //        _context.TestRunResults.Add(
            //            new TestRunResult
            //            {
            //                TestCaseId = resultDto.TestCaseId,
            //                TestRunId = testRunId,
            //                Status = resultDto.Status,
            //                Comments = resultDto.Comments,
            //                Attachments = resultDto.Attachments,
            //                TenantId = tenantId
            //            }
            //        );
            //    }
            //}

            await _context.SaveChangesAsync();
        }
    }
}
