using EffortlessQA.Api.Services.Interface;
using EffortlessQA.Data;
using EffortlessQA.Data.Dtos;
using EffortlessQA.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EffortlessQA.Api.Extensions.Endpoints
{
    public class TestFolderService : ITestFolderService
    {
        private readonly EffortlessQAContext _context;
        private readonly IConfiguration _configuration;

        public TestFolderService(EffortlessQAContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<TestFolderDto> CreateTestFolderAsync(
            Guid projectId,
            string tenantId,
            CreateTestFolderDto dto
        )
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p =>
                p.Id == projectId && p.TenantId == tenantId && !p.IsDeleted
            );

            if (project == null)
                throw new Exception("Project not found.");

            var testFolder = new TestFolder
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                ProjectId = projectId,
                TenantId = tenantId,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };

            await _context.TestFolders.AddAsync(testFolder);
            await _context.SaveChangesAsync();

            return new TestFolderDto
            {
                Id = testFolder.Id,
                Name = testFolder.Name,
                Description = testFolder.Description,
                ProjectId = testFolder.ProjectId,
                TenantId = testFolder.TenantId,
                CreatedAt = testFolder.CreatedAt,
                UpdatedAt = testFolder.ModifiedAt
            };
        }

        public async Task<PagedResult<TestFolderDto>> GetTestFoldersAsync(
            Guid projectId,
            string tenantId,
            int page,
            int limit,
            string? filter
        )
        {
            var query = _context.TestFolders.Where(tf =>
                tf.ProjectId == projectId && tf.TenantId == tenantId && !tf.IsDeleted
            );

            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(tf => tf.Name.Contains(filter));
            }

            query = query.OrderBy(tf => tf.Name);

            var totalCount = await query.CountAsync();
            var testFolders = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(tf => new TestFolderDto
                {
                    Id = tf.Id,
                    Name = tf.Name,
                    Description = tf.Description,
                    ProjectId = tf.ProjectId,
                    TenantId = tf.TenantId,
                    CreatedAt = tf.CreatedAt,
                    UpdatedAt = tf.ModifiedAt
                })
                .ToListAsync();

            return new PagedResult<TestFolderDto>
            {
                Items = testFolders,
                TotalCount = totalCount,
                Page = page,
                Limit = limit
            };
        }

        public async Task<TestFolderDto> GetTestFolderAsync(
            Guid folderId,
            Guid projectId,
            string tenantId
        )
        {
            var testFolder = await _context.TestFolders.FirstOrDefaultAsync(tf =>
                tf.Id == folderId
                && tf.ProjectId == projectId
                && tf.TenantId == tenantId
                && !tf.IsDeleted
            );

            if (testFolder == null)
                throw new Exception("Test folder not found.");

            return new TestFolderDto
            {
                Id = testFolder.Id,
                Name = testFolder.Name,
                Description = testFolder.Description,
                ProjectId = testFolder.ProjectId,
                TenantId = testFolder.TenantId,
                CreatedAt = testFolder.CreatedAt,
                UpdatedAt = testFolder.ModifiedAt
            };
        }

        public async Task<TestFolderDto> UpdateTestFolderAsync(
            Guid folderId,
            Guid projectId,
            string tenantId,
            UpdateTestFolderDto dto
        )
        {
            var testFolder = await _context.TestFolders.FirstOrDefaultAsync(tf =>
                tf.Id == folderId
                && tf.ProjectId == projectId
                && tf.TenantId == tenantId
                && !tf.IsDeleted
            );

            if (testFolder == null)
                throw new Exception("Test folder not found.");

            testFolder.Name = dto.Name ?? testFolder.Name;
            testFolder.Description = dto.Description ?? testFolder.Description;
            testFolder.ModifiedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new TestFolderDto
            {
                Id = testFolder.Id,
                Name = testFolder.Name,
                Description = testFolder.Description,
                ProjectId = testFolder.ProjectId,
                TenantId = testFolder.TenantId,
                CreatedAt = testFolder.CreatedAt,
                UpdatedAt = testFolder.ModifiedAt
            };
        }

        public async Task DeleteTestFolderAsync(Guid folderId, Guid projectId, string tenantId)
        {
            var testFolder = await _context.TestFolders.FirstOrDefaultAsync(tf =>
                tf.Id == folderId
                && tf.ProjectId == projectId
                && tf.TenantId == tenantId
                && !tf.IsDeleted
            );

            if (testFolder == null)
                throw new Exception("Test folder not found.");

            testFolder.IsDeleted = true;
            testFolder.ModifiedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
