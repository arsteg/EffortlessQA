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
            Steps = dto.Steps,
            ExpectedResults = dto.ExpectedResults,
            Priority = dto.Priority,
            Tags = dto.Tags,
            TestSuiteId = testSuiteId,
            TenantId = tenantId,
            FolderId = dto.FolderId,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow,
            ActualResult = dto.ActualResult,
            Comments = dto.Comments,
            TestData = dto.TestData,
            Precondition = dto.Precondition,
            Status = dto.Status,
            Screenshot = dto.Screenshot
        };

        try
        {
            await _context.TestCases.AddAsync(testCase);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            var innerException = ex.InnerException?.Message ?? ex.Message;
            throw new Exception($"Failed to save test case: {innerException}", ex);
        }

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
            UpdatedAt = testCase.ModifiedAt,
            ActualResult = testCase.ActualResult,
            Comments = testCase.Comments,
            TestData = testCase.TestData,
            Precondition = testCase.Precondition,
            Status = testCase.Status,
            Screenshot = testCase.Screenshot
        };
    }

    public async Task<PagedResult<TestCaseDto>> GetTestCasesAsync(
        string tenantId,
        int page,
        int limit,
        string? filter,
        string[]? tags,
        PriorityLevel[]? priorities
    )
    {
        var query = _context.TestCases.Where(tc => tc.TenantId == tenantId && !tc.IsDeleted);

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

        if (!string.IsNullOrEmpty(titleFilter))
        {
            query = query.Where(tc => tc.Title.ToLower().Contains(titleFilter.ToLower()));
        }

        if (tags != null && tags.Length > 0)
        {
            query = query.Where(tc => tc.Tags != null && tags.Any(t => tc.Tags.Contains(t)));
        }

        if (priorities != null && priorities.Length > 0)
        {
            query = query.Where(tc => priorities.Contains(tc.Priority));
        }

        if (sortField == "title")
        {
            query = sortAscending
                ? query.OrderBy(tc => tc.Title)
                : query.OrderByDescending(tc => tc.Title);
        }
        else if (sortField == "description")
        {
            query = sortAscending
                ? query.OrderBy(tc => tc.Description)
                : query.OrderByDescending(tc => tc.Description);
        }
        else if (sortField == "createdat")
        {
            query = sortAscending
                ? query.OrderBy(tc => tc.CreatedAt)
                : query.OrderByDescending(tc => tc.CreatedAt);
        }
        else if (sortField == "updatedat")
        {
            query = sortAscending
                ? query.OrderBy(tc => tc.ModifiedAt)
                : query.OrderByDescending(tc => tc.ModifiedAt);
        }
        else
        {
            query = query.OrderBy(tc => tc.Title);
        }

        var totalCount = await query.CountAsync();

        var testCases = await query
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(tc => new TestCaseDto
            {
                Id = tc.Id,
                Title = tc.Title,
                Description = tc.Description,
                Steps = tc.Steps != null ? tc.Steps.ToString() : string.Empty,
                ExpectedResults =
                    tc.ExpectedResults != null ? tc.ExpectedResults.ToString() : string.Empty,
                Priority = tc.Priority,
                Tags = tc.Tags,
                TestSuiteId = tc.TestSuiteId,
                TenantId = tc.TenantId,
                //FolderId = tc.FolderId,
                CreatedAt = tc.CreatedAt,
                UpdatedAt = tc.ModifiedAt,
                ActualResult = tc.ActualResult,
                Comments = tc.Comments,
                TestData = tc.TestData,
                Precondition = tc.Precondition,
                Status = tc.Status,
                Screenshot = tc.Screenshot
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

        if (!string.IsNullOrEmpty(titleFilter))
        {
            query = query.Where(tc => tc.Title.ToLower().Contains(titleFilter.ToLower()));
        }

        if (tags != null && tags.Length > 0)
        {
            query = query.Where(tc => tc.Tags != null && tags.Any(t => tc.Tags.Contains(t)));
        }

        if (priorities != null && priorities.Length > 0)
        {
            query = query.Where(tc => priorities.Contains(tc.Priority));
        }

        if (sortField == "title")
        {
            query = sortAscending
                ? query.OrderBy(tc => tc.Title)
                : query.OrderByDescending(tc => tc.Title);
        }
        else if (sortField == "description")
        {
            query = sortAscending
                ? query.OrderBy(tc => tc.Description)
                : query.OrderByDescending(tc => tc.Description);
        }
        else if (sortField == "createdat")
        {
            query = sortAscending
                ? query.OrderBy(tc => tc.CreatedAt)
                : query.OrderByDescending(tc => tc.CreatedAt);
        }
        else if (sortField == "updatedat")
        {
            query = sortAscending
                ? query.OrderBy(tc => tc.ModifiedAt)
                : query.OrderByDescending(tc => tc.ModifiedAt);
        }
        else
        {
            query = query.OrderBy(tc => tc.Title);
        }

        var totalCount = await query.CountAsync();

        var testCases = await query
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(tc => new TestCaseDto
            {
                Id = tc.Id,
                Title = tc.Title,
                Description = tc.Description,
                Steps = tc.Steps != null ? tc.Steps.ToString() : string.Empty,
                ExpectedResults =
                    tc.ExpectedResults != null ? tc.ExpectedResults.ToString() : string.Empty,
                Priority = tc.Priority,
                Tags = tc.Tags,
                TestSuiteId = tc.TestSuiteId,
                TenantId = tc.TenantId,
                //FolderId = tc.FolderId,
                CreatedAt = tc.CreatedAt,
                UpdatedAt = tc.ModifiedAt,
                ActualResult = tc.ActualResult,
                Comments = tc.Comments,
                TestData = tc.TestData,
                Precondition = tc.Precondition,
                Status = tc.Status,
                Screenshot = tc.Screenshot
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
            Steps = testCase.Steps != null ? testCase.Steps.ToString() : string.Empty,
            ExpectedResults =
                testCase.ExpectedResults != null
                    ? testCase.ExpectedResults.ToString()
                    : string.Empty,
            Priority = testCase.Priority,
            Tags = testCase.Tags,
            TestSuiteId = testCase.TestSuiteId,
            TenantId = testCase.TenantId,
            //FolderId = testCase.FolderId,
            CreatedAt = testCase.CreatedAt,
            UpdatedAt = testCase.ModifiedAt,
            ActualResult = testCase.ActualResult,
            Comments = testCase.Comments,
            TestData = testCase.TestData,
            Precondition = testCase.Precondition,
            Status = testCase.Status,
            Screenshot = testCase.Screenshot
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
        testCase.Steps = dto.Steps;
        testCase.ExpectedResults = testCase.ExpectedResults;

        testCase.Priority = dto.Priority ?? testCase.Priority;
        testCase.Tags = dto.Tags ?? testCase.Tags;
        testCase.FolderId = dto.FolderId ?? testCase.FolderId;
        testCase.ActualResult = dto.ActualResult ?? testCase.ActualResult;
        testCase.Comments = dto.Comments ?? testCase.Comments;
        testCase.TestData = dto.TestData ?? testCase.TestData;
        testCase.Precondition = dto.Precondition ?? testCase.Precondition;
        testCase.Status = dto.Status ?? testCase.Status;
        testCase.Screenshot = dto.Screenshot ?? testCase.Screenshot;
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
            UpdatedAt = testCase.ModifiedAt,
            ActualResult = testCase.ActualResult,
            Comments = testCase.Comments,
            TestData = testCase.TestData,
            Precondition = testCase.Precondition,
            Status = testCase.Status,
            Screenshot = testCase.Screenshot
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
            Steps = sourceTestCase.Steps,
            ExpectedResults = sourceTestCase.ExpectedResults,
            Priority = sourceTestCase.Priority,
            Tags = sourceTestCase.Tags,
            TestSuiteId = dto.TargetTestSuiteId,
            TenantId = tenantId,
            FolderId = dto.TargetFolderId,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow,
            ActualResult = sourceTestCase.ActualResult,
            Comments = sourceTestCase.Comments,
            TestData = sourceTestCase.TestData,
            Precondition = sourceTestCase.Precondition,
            Status = sourceTestCase.Status,
            Screenshot = sourceTestCase.Screenshot
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
            UpdatedAt = newTestCase.ModifiedAt,
            ActualResult = newTestCase.ActualResult,
            Comments = newTestCase.Comments,
            TestData = newTestCase.TestData,
            Precondition = newTestCase.Precondition,
            Status = newTestCase.Status,
            Screenshot = newTestCase.Screenshot
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

        using (var stream = file.OpenReadStream())
        using (var reader = new StreamReader(stream))
        {
            await reader.ReadLineAsync();

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                var values = line.Split(',');

                if (values.Length >= 11)
                {
                    var testCase = new TestCase
                    {
                        Id = Guid.NewGuid(),
                        Title = values[0].Trim(),
                        Description = string.IsNullOrEmpty(values[1]) ? null : values[1].Trim(),
                        Steps = values[2],
                        ExpectedResults = values[3].Trim(),
                        Priority = Enum.Parse<PriorityLevel>(values[4].Trim(), true),
                        Tags =
                            values.Length > 5 && !string.IsNullOrEmpty(values[5])
                                ? values[5].Split(';')
                                : null,
                        TestSuiteId = testSuiteId,
                        TenantId = tenantId,
                        CreatedAt = DateTime.UtcNow,
                        ModifiedAt = DateTime.UtcNow,
                        ActualResult = string.IsNullOrEmpty(values[6]) ? null : values[6].Trim(),
                        Comments = string.IsNullOrEmpty(values[7]) ? null : values[7].Trim(),
                        TestData = string.IsNullOrEmpty(values[8]) ? null : values[8].Trim(),
                        Precondition = string.IsNullOrEmpty(values[9]) ? null : values[9].Trim(),
                        Status = string.IsNullOrEmpty(values[10])
                            ? null
                            : Enum.Parse<TestExecutionStatus>(values[10].Trim(), true),
                        Screenshot = string.IsNullOrEmpty(values[11]) ? null : values[11].Trim()
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
                            UpdatedAt = testCase.ModifiedAt,
                            ActualResult = testCase.ActualResult,
                            Comments = testCase.Comments,
                            TestData = testCase.TestData,
                            Precondition = testCase.Precondition,
                            Status = testCase.Status,
                            Screenshot = testCase.Screenshot
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
                Steps = tc.Steps.ToString() ?? string.Empty,
                ExpectedResults = tc.ExpectedResults.ToString() ?? string.Empty,
                Priority = tc.Priority,
                Tags = tc.Tags,
                TestSuiteId = tc.TestSuiteId,
                TenantId = tc.TenantId,
                //FolderId = tc.FolderId,
                CreatedAt = tc.CreatedAt,
                UpdatedAt = tc.ModifiedAt,
                ActualResult = tc.ActualResult,
                Comments = tc.Comments,
                TestData = tc.TestData,
                Precondition = tc.Precondition,
                Status = tc.Status,
                Screenshot = tc.Screenshot
            })
            .ToListAsync();

        var csvBuilder = new StringBuilder();
        csvBuilder.AppendLine(
            "Title,Description,Steps,ExpectedResults,Priority,Tags,ActualResult,Comments,TestData,Precondition,Status,Screenshot"
        );

        foreach (var tc in testCases)
        {
            var tags = tc.Tags != null ? string.Join(";", tc.Tags) : "";
            csvBuilder.AppendLine(
                $"\"{tc.Title}\",\"{tc.Description ?? ""}\",\"{tc.Steps ?? ""}\",\"{tc.ExpectedResults ?? ""}\",\"{tc.Priority}\",\"{tags}\",\"{tc.ActualResult ?? ""}\",\"{tc.Comments ?? ""}\",\"{tc.TestData ?? ""}\",\"{tc.Precondition ?? ""}\",\"{tc.Status?.ToString() ?? ""}\",\"{tc.Screenshot ?? ""}\""
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
            UpdatedAt = testCase.ModifiedAt,
            ActualResult = testCase.ActualResult,
            Comments = testCase.Comments,
            TestData = testCase.TestData,
            Precondition = testCase.Precondition,
            Status = testCase.Status,
            Screenshot = testCase.Screenshot
        };
    }
}
