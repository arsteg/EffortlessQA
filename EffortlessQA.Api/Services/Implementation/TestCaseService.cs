using System.Text;
using System.Text.Json;
using EffortlessQA.Api.Services.Interface;
using EffortlessQA.Data;
using EffortlessQA.Data.Dtos;
using EffortlessQA.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

public class TestCaseService : ITestCaseService
{
    private readonly EffortlessQAContext _context;
    private readonly IConfiguration _configuration;

    public TestCaseService(EffortlessQAContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<TestCaseDto> CreateTestCaseAsync(
        Guid testSuiteId,
        string tenantId,
        CreateTestCaseDto dto
    )
    {
        var testSuite = await _context.TestSuites.FirstOrDefaultAsync(ts =>
            ts.Id == testSuiteId && ts.TenantId == tenantId && !ts.IsDeleted
        );

        if (testSuite == null)
            throw new Exception("Test suite not found.");

        if (dto.FolderId.HasValue)
        {
            var folder = await _context.TestFolders.FirstOrDefaultAsync(tf =>
                tf.Id == dto.FolderId
                && tf.ProjectId == testSuite.ProjectId
                && tf.TenantId == tenantId
                && !tf.IsDeleted
            );
            if (folder == null)
                throw new Exception("Test folder not found.");
        }

        var testCase = new TestCase
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            Steps = dto.Steps != null ? JsonDocument.Parse(dto.Steps) : null,
            ExpectedResults =
                dto.ExpectedResults != null ? JsonDocument.Parse(dto.ExpectedResults) : null,
            Priority = dto.Priority,
            Tags = dto.Tags,
            TestSuiteId = testSuiteId,
            TenantId = tenantId,
            FolderId = dto.FolderId,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow
        };

        await _context.TestCases.AddAsync(testCase);
        await _context.SaveChangesAsync();

        return new TestCaseDto
        {
            Id = testCase.Id,
            Title = testCase.Title,
            Description = testCase.Description,
            Steps = testCase.Steps?.ToString(),
            ExpectedResults = testCase.ExpectedResults?.ToString(),
            Priority = testCase.Priority,
            Tags = testCase.Tags,
            TestSuiteId = testCase.TestSuiteId,
            TenantId = testCase.TenantId,
            //FolderId = testCase.FolderId,
            CreatedAt = testCase.CreatedAt,
            UpdatedAt = testCase.ModifiedAt
        };
    }

    public async Task<PagedResult<TestCaseDto>> GetTestCasesAsync(
        Guid testSuiteId,
        string tenantId,
        int page,
        int limit,
        string? filter,
        string[]? tags,
        PriorityLevel[]? priorities
    )
    {
        var query = _context.TestCases.Where(tc =>
            tc.TestSuiteId == testSuiteId && tc.TenantId == tenantId && !tc.IsDeleted
        );

        if (!string.IsNullOrEmpty(filter))
        {
            query = query.Where(tc => tc.Title.Contains(filter));
        }

        if (tags != null && tags.Length > 0)
        {
            query = query.Where(tc => tc.Tags != null && tc.Tags.Any(t => tags.Contains(t)));
        }

        if (priorities != null && priorities.Length > 0)
        {
            query = query.Where(tc => priorities.Contains(tc.Priority));
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
                //Steps = tc.Steps?.ToString(),
                //ExpectedResults = tc.ExpectedResults?.ToString(),
                Priority = tc.Priority,
                Tags = tc.Tags,
                TestSuiteId = tc.TestSuiteId,
                TenantId = tc.TenantId,
                //FolderId = tc.FolderId,
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

    public async Task<TestCaseDto> GetTestCaseAsync(
        Guid testCaseId,
        Guid testSuiteId,
        string tenantId
    )
    {
        var testCase = await _context.TestCases.FirstOrDefaultAsync(tc =>
            tc.Id == testCaseId
            && tc.TestSuiteId == testSuiteId
            && tc.TenantId == tenantId
            && !tc.IsDeleted
        );

        if (testCase == null)
            throw new Exception("Test case not found.");

        return new TestCaseDto
        {
            Id = testCase.Id,
            Title = testCase.Title,
            Description = testCase.Description,
            Steps = testCase.Steps?.ToString(),
            ExpectedResults = testCase.ExpectedResults?.ToString(),
            Priority = testCase.Priority,
            Tags = testCase.Tags,
            TestSuiteId = testCase.TestSuiteId,
            TenantId = testCase.TenantId,
            //FolderId = testCase.FolderId,
            CreatedAt = testCase.CreatedAt,
            UpdatedAt = testCase.ModifiedAt
        };
    }

    public async Task<TestCaseDto> UpdateTestCaseAsync(
        Guid testCaseId,
        Guid testSuiteId,
        string tenantId,
        UpdateTestCaseDto dto
    )
    {
        var testCase = await _context.TestCases.FirstOrDefaultAsync(tc =>
            tc.Id == testCaseId
            && tc.TestSuiteId == testSuiteId
            && tc.TenantId == tenantId
            && !tc.IsDeleted
        );

        if (testCase == null)
            throw new Exception("Test case not found.");

        if (dto.FolderId.HasValue)
        {
            var folder = await _context.TestFolders.FirstOrDefaultAsync(tf =>
                tf.Id == dto.FolderId
                && tf.ProjectId == testCase.TestSuite.ProjectId
                && tf.TenantId == tenantId
                && !tf.IsDeleted
            );
            if (folder == null)
                throw new Exception("Test folder not found.");
        }

        testCase.Title = dto.Title ?? testCase.Title;
        testCase.Description = dto.Description ?? testCase.Description;
        testCase.Steps = dto.Steps != null ? JsonDocument.Parse(dto.Steps) : testCase.Steps;
        testCase.ExpectedResults =
            dto.ExpectedResults != null
                ? JsonDocument.Parse(dto.ExpectedResults)
                : testCase.ExpectedResults;
        testCase.Priority = dto.Priority ?? testCase.Priority;
        testCase.Tags = dto.Tags ?? testCase.Tags;
        testCase.FolderId = dto.FolderId ?? testCase.FolderId;
        testCase.ModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return new TestCaseDto
        {
            Id = testCase.Id,
            Title = testCase.Title,
            Description = testCase.Description,
            Steps = testCase.Steps?.ToString(),
            ExpectedResults = testCase.ExpectedResults?.ToString(),
            Priority = testCase.Priority,
            Tags = testCase.Tags,
            TestSuiteId = testCase.TestSuiteId,
            TenantId = testCase.TenantId,
            //FolderId = testCase.FolderId,
            CreatedAt = testCase.CreatedAt,
            UpdatedAt = testCase.ModifiedAt
        };
    }

    public async Task DeleteTestCaseAsync(Guid testCaseId, Guid testSuiteId, string tenantId)
    {
        var testCase = await _context.TestCases.FirstOrDefaultAsync(tc =>
            tc.Id == testCaseId
            && tc.TestSuiteId == testSuiteId
            && tc.TenantId == tenantId
            && !tc.IsDeleted
        );

        if (testCase == null)
            throw new Exception("Test case not found.");

        testCase.IsDeleted = true;
        testCase.ModifiedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task<TestCaseDto> CopyTestCaseAsync(
        Guid testCaseId,
        string tenantId,
        CopyTestCaseDto dto
    )
    {
        var sourceTestCase = await _context.TestCases.FirstOrDefaultAsync(tc =>
            tc.Id == testCaseId && tc.TenantId == tenantId && !tc.IsDeleted
        );

        if (sourceTestCase == null)
            throw new Exception("Source test case not found.");

        var targetTestSuite = await _context.TestSuites.FirstOrDefaultAsync(ts =>
            ts.Id == dto.TargetTestSuiteId && ts.TenantId == tenantId && !ts.IsDeleted
        );

        if (targetTestSuite == null)
            throw new Exception("Target test suite not found.");

        if (dto.TargetFolderId.HasValue)
        {
            var folder = await _context.TestFolders.FirstOrDefaultAsync(tf =>
                tf.Id == dto.TargetFolderId
                && tf.ProjectId == targetTestSuite.ProjectId
                && tf.TenantId == tenantId
                && !tf.IsDeleted
            );
            if (folder == null)
                throw new Exception("Target test folder not found.");
        }

        var newTestCase = new TestCase
        {
            Id = Guid.NewGuid(),
            Title = sourceTestCase.Title + " (Copy)",
            Description = sourceTestCase.Description,
            Steps =
                sourceTestCase.Steps != null
                    ? JsonDocument.Parse(sourceTestCase.Steps.ToString())
                    : null,
            ExpectedResults =
                sourceTestCase.ExpectedResults != null
                    ? JsonDocument.Parse(sourceTestCase.ExpectedResults.ToString())
                    : null,
            Priority = sourceTestCase.Priority,
            Tags = sourceTestCase.Tags,
            TestSuiteId = dto.TargetTestSuiteId,
            TenantId = tenantId,
            FolderId = dto.TargetFolderId,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow
        };

        await _context.TestCases.AddAsync(newTestCase);
        await _context.SaveChangesAsync();

        return new TestCaseDto
        {
            Id = newTestCase.Id,
            Title = newTestCase.Title,
            Description = newTestCase.Description,
            Steps = newTestCase.Steps?.ToString(),
            ExpectedResults = newTestCase.ExpectedResults?.ToString(),
            Priority = newTestCase.Priority,
            Tags = newTestCase.Tags,
            TestSuiteId = newTestCase.TestSuiteId,
            TenantId = newTestCase.TenantId,
            //FolderId = newTestCase.FolderId,
            CreatedAt = newTestCase.CreatedAt,
            UpdatedAt = newTestCase.ModifiedAt
        };
    }

    public async Task<IList<TestCaseDto>> ImportTestCasesAsync(
        Guid testSuiteId,
        string tenantId,
        IFormFile file
    )
    {
        var testSuite = await _context.TestSuites.FirstOrDefaultAsync(ts =>
            ts.Id == testSuiteId && ts.TenantId == tenantId && !ts.IsDeleted
        );

        if (testSuite == null)
            throw new Exception("Test suite not found.");

        if (file == null || file.Length == 0)
            throw new Exception("No file uploaded.");

        var testCases = new List<TestCase>();
        var testCaseDtos = new List<TestCaseDto>();

        // Simplified CSV/Excel parsing (use a library like CsvHelper or EPPlus in production)
        using (var stream = file.OpenReadStream())
        using (var reader = new StreamReader(stream))
        {
            // Skip header
            await reader.ReadLineAsync();

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                var values = line.Split(',');

                if (values.Length >= 5)
                {
                    var testCase = new TestCase
                    {
                        Id = Guid.NewGuid(),
                        Title = values[0].Trim(),
                        Description = string.IsNullOrEmpty(values[1]) ? null : values[1].Trim(),
                        Steps = string.IsNullOrEmpty(values[2])
                            ? null
                            : JsonDocument.Parse(values[2].Trim()),
                        ExpectedResults = string.IsNullOrEmpty(values[3])
                            ? null
                            : JsonDocument.Parse(values[3].Trim()),
                        Priority = Enum.Parse<PriorityLevel>(values[4].Trim(), true),
                        Tags =
                            values.Length > 5 && !string.IsNullOrEmpty(values[5])
                                ? values[5].Split(';')
                                : null,
                        TestSuiteId = testSuiteId,
                        TenantId = tenantId,
                        CreatedAt = DateTime.UtcNow,
                        ModifiedAt = DateTime.UtcNow
                    };

                    testCases.Add(testCase);
                    testCaseDtos.Add(
                        new TestCaseDto
                        {
                            Id = testCase.Id,
                            Title = testCase.Title,
                            Description = testCase.Description,
                            Steps = testCase.Steps?.ToString(),
                            ExpectedResults = testCase.ExpectedResults?.ToString(),
                            Priority = testCase.Priority,
                            Tags = testCase.Tags,
                            TestSuiteId = testCase.TestSuiteId,
                            TenantId = testCase.TenantId,
                            //FolderId = testCase.FolderId,
                            CreatedAt = testCase.CreatedAt,
                            UpdatedAt = testCase.ModifiedAt
                        }
                    );
                }
            }
        }

        await _context.TestCases.AddRangeAsync(testCases);
        await _context.SaveChangesAsync();

        return testCaseDtos;
    }

    public async Task<byte[]> ExportTestCasesAsync(Guid testSuiteId, string tenantId)
    {
        var testCases = await _context
            .TestCases.Where(tc =>
                tc.TestSuiteId == testSuiteId && tc.TenantId == tenantId && !tc.IsDeleted
            )
            .Select(tc => new TestCaseDto
            {
                Id = tc.Id,
                Title = tc.Title,
                Description = tc.Description,
                //Steps = tc.Steps?.ToString(),
                //ExpectedResults = tc.ExpectedResults?.ToString(),
                Priority = tc.Priority,
                Tags = tc.Tags,
                TestSuiteId = tc.TestSuiteId,
                TenantId = tc.TenantId,
                //FolderId = tc.FolderId,
                CreatedAt = tc.CreatedAt,
                UpdatedAt = tc.ModifiedAt
            })
            .ToListAsync();

        var csvBuilder = new StringBuilder();
        csvBuilder.AppendLine("Title,Description,Steps,ExpectedResults,Priority,Tags");

        foreach (var tc in testCases)
        {
            var tags = tc.Tags != null ? string.Join(";", tc.Tags) : "";
            csvBuilder.AppendLine(
                $"\"{tc.Title}\",\"{tc.Description ?? ""}\",\"{tc.Steps ?? ""}\",\"{tc.ExpectedResults ?? ""}\",\"{tc.Priority}\",\"{tags}\""
            );
        }

        return Encoding.UTF8.GetBytes(csvBuilder.ToString());
    }

    public async Task<TestCaseDto> MoveTestCaseAsync(
        Guid testCaseId,
        string tenantId,
        MoveTestCaseDto dto
    )
    {
        var testCase = await _context.TestCases.FirstOrDefaultAsync(tc =>
            tc.Id == testCaseId && tc.TenantId == tenantId && !tc.IsDeleted
        );

        if (testCase == null)
            throw new Exception("Test case not found.");

        var targetTestSuite = await _context.TestSuites.FirstOrDefaultAsync(ts =>
            ts.Id == dto.TargetTestSuiteId && ts.TenantId == tenantId && !ts.IsDeleted
        );

        if (targetTestSuite == null)
            throw new Exception("Target test suite not found.");

        if (dto.TargetFolderId.HasValue)
        {
            var folder = await _context.TestFolders.FirstOrDefaultAsync(tf =>
                tf.Id == dto.TargetFolderId
                && tf.ProjectId == targetTestSuite.ProjectId
                && tf.TenantId == tenantId
                && !tf.IsDeleted
            );
            if (folder == null)
                throw new Exception("Target test folder not found.");
        }

        testCase.TestSuiteId = dto.TargetTestSuiteId;
        testCase.FolderId = dto.TargetFolderId;
        testCase.ModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return new TestCaseDto
        {
            Id = testCase.Id,
            Title = testCase.Title,
            Description = testCase.Description,
            Steps = testCase.Steps?.ToString(),
            ExpectedResults = testCase.ExpectedResults?.ToString(),
            Priority = testCase.Priority,
            Tags = testCase.Tags,
            TestSuiteId = testCase.TestSuiteId,
            TenantId = testCase.TenantId,
            //FolderId = testCase.FolderId,
            CreatedAt = testCase.CreatedAt,
            UpdatedAt = testCase.ModifiedAt
        };
    }
}
