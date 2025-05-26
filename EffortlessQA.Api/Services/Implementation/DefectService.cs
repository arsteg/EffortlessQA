using System.Text.Json;
using EffortlessQA.Api.Services.Interface;
using EffortlessQA.Data;
using EffortlessQA.Data.Dtos;
using EffortlessQA.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EffortlessQA.Api.Services.Implementation
{
    public class DefectService : IDefectService
    {
        private readonly EffortlessQAContext _context;
        private readonly IConfiguration _configuration;

        public DefectService(EffortlessQAContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<DefectDto> CreateDefectAsync(string tenantId, CreateDefectDto dto)
        {
            if (dto.TestRunResultId.HasValue)
            {
                var testRunResult = await _context.TestRunResults.FirstOrDefaultAsync(trr =>
                    trr.Id == dto.TestRunResultId && trr.TenantId == tenantId
                );
                if (testRunResult == null)
                    throw new Exception("Test run result not found.");
            }

            if (dto.TestCaseId.HasValue)
            {
                var testCase = await _context.TestCases.FirstOrDefaultAsync(tc =>
                    tc.Id == dto.TestCaseId && tc.TenantId == tenantId && !tc.IsDeleted
                );
                if (testCase == null)
                    throw new Exception("Test case not found.");
            }

            if (dto.AssignedUserId.HasValue)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u =>
                    u.Id == dto.AssignedUserId && u.TenantId == tenantId && !u.IsDeleted
                );
                if (user == null)
                    throw new Exception("Assigned user not found.");
            }

            var defect = new Defect
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                Severity = dto.Severity,
                Status = dto.Status,
                // Attachments = dto.Attachments != null ? JsonDocument.Parse(dto.Attachments) : null,
                ExternalId = dto.ExternalId,
                TestRunResultId = dto.TestRunResultId,
                TestCaseId = dto.TestCaseId,
                TenantId = tenantId,
                AssignedUserId = dto.AssignedUserId,
                ResolutionNotes = dto.ResolutionNotes,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };

            await _context.Defects.AddAsync(defect);
            await _context.SaveChangesAsync();

            // Log history for creation
            await LogDefectHistoryAsync(
                defect.Id,
                tenantId,
                "Created",
                $"Defect created with status {dto.Status}"
            );

            return new DefectDto
            {
                Id = defect.Id,
                Title = defect.Title,
                Description = defect.Description,
                Severity = defect.Severity,
                Status = defect.Status,
                Attachments = defect.Attachments?.ToString(),
                ExternalId = defect.ExternalId,
                TestRunResultId = defect.TestRunResultId,
                TestCaseId = defect.TestCaseId,
                TenantId = defect.TenantId,
                AssignedUserId = defect.AssignedUserId,
                ResolutionNotes = defect.ResolutionNotes,
                CreatedAt = defect.CreatedAt,
                UpdatedAt = defect.ModifiedAt
            };
        }

        public async Task<PagedResult<DefectDto>> GetDefectsAsync(
            string tenantId,
            int page,
            int limit,
            string? filter,
            SeverityLevel[]? severities,
            DefectStatus[]? statuses
        )
        {
            var query = _context.Defects.Where(d => d.TenantId == tenantId && !d.IsDeleted);

            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(d => d.Title.Contains(filter));
            }

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
                    //Attachments = d.Attachments?.ToString(),
                    ExternalId = d.ExternalId,
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

        public async Task<DefectDto> GetDefectAsync(Guid defectId, string tenantId)
        {
            var defect = await _context.Defects.FirstOrDefaultAsync(d =>
                d.Id == defectId && d.TenantId == tenantId && !d.IsDeleted
            );

            if (defect == null)
                throw new Exception("Defect not found.");

            return new DefectDto
            {
                Id = defect.Id,
                Title = defect.Title,
                Description = defect.Description,
                Severity = defect.Severity,
                Status = defect.Status,
                Attachments = defect.Attachments?.ToString(),
                ExternalId = defect.ExternalId,
                TestRunResultId = defect.TestRunResultId,
                TestCaseId = defect.TestCaseId,
                TenantId = defect.TenantId,
                AssignedUserId = defect.AssignedUserId,
                ResolutionNotes = defect.ResolutionNotes,
                CreatedAt = defect.CreatedAt,
                UpdatedAt = defect.ModifiedAt
            };
        }

        public async Task<DefectDto> UpdateDefectAsync(
            Guid defectId,
            string tenantId,
            UpdateDefectDto dto
        )
        {
            var defect = await _context.Defects.FirstOrDefaultAsync(d =>
                d.Id == defectId && d.TenantId == tenantId && !d.IsDeleted
            );

            if (defect == null)
                throw new Exception("Defect not found.");

            //if (dto.TestRunResultId.HasValue)
            //{
            //    var testRunResult = await _context.TestRunResults.FirstOrDefaultAsync(trr =>
            //        trr.Id == dto.TestRunResultId && trr.TenantId == tenantId
            //    );
            //    if (testRunResult == null)
            //        throw new Exception("Test run result not found.");
            //}

            //if (dto.TestCaseId.HasValue)
            //{
            //    var testCase = await _context.TestCases.FirstOrDefaultAsync(tc =>
            //        tc.Id == dto.TestCaseId && tc.TenantId == tenantId && !tc.IsDeleted
            //    );
            //    if (testCase == null)
            //        throw new Exception("Test case not found.");
            //}

            if (dto.AssignedUserId.HasValue)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u =>
                    u.Id == dto.AssignedUserId && u.TenantId == tenantId && !u.IsDeleted
                );
                if (user == null)
                    throw new Exception("Assigned user not found.");
            }

            var oldStatus = defect.Status;
            defect.Title = dto.Title ?? defect.Title;
            defect.Description = dto.Description ?? defect.Description;
            //defect.Severity = dto.Severity ?? defect.Severity;
            //defect.Status = dto.Status ?? defect.Status;
            //defect.Attachments =  dto.Attachments != null ? JsonDocument.Parse(dto.Attachments) : defect.Attachments;
            //defect.ExternalId = dto.ExternalId ?? defect.ExternalId;
            //defect.TestRunResultId = dto.TestRunResultId ?? defect.TestRunResultId;
            //defect.TestCaseId = dto.TestCaseId ?? defect.TestCaseId;
            //defect.AssignedUserId = dto.AssignedUserId ?? defect.AssignedUserId;
            //defect.ResolutionNotes = dto.ResolutionNotes ?? defect.ResolutionNotes;
            //defect.ModifiedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Log history for status change or comments
            if (dto.Status != null && dto.Status != oldStatus)
            {
                await LogDefectHistoryAsync(
                    defect.Id,
                    tenantId,
                    "Status Changed",
                    $"Status changed from {oldStatus} to {dto.Status}"
                );
            }
            if (dto.ResolutionNotes != null && dto.ResolutionNotes != defect.ResolutionNotes)
            {
                await LogDefectHistoryAsync(
                    defect.Id,
                    tenantId,
                    "Resolution Notes Updated",
                    "Resolution notes updated"
                );
            }

            return new DefectDto
            {
                Id = defect.Id,
                Title = defect.Title,
                Description = defect.Description,
                Severity = defect.Severity,
                Status = defect.Status,
                Attachments = defect.Attachments?.ToString(),
                ExternalId = defect.ExternalId,
                TestRunResultId = defect.TestRunResultId,
                TestCaseId = defect.TestCaseId,
                TenantId = defect.TenantId,
                AssignedUserId = defect.AssignedUserId,
                ResolutionNotes = defect.ResolutionNotes,
                CreatedAt = defect.CreatedAt,
                UpdatedAt = defect.ModifiedAt
            };
        }

        public async Task DeleteDefectAsync(Guid defectId, string tenantId)
        {
            var defect = await _context.Defects.FirstOrDefaultAsync(d =>
                d.Id == defectId && d.TenantId == tenantId && !d.IsDeleted
            );

            if (defect == null)
                throw new Exception("Defect not found.");

            defect.IsDeleted = true;
            defect.ModifiedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            await LogDefectHistoryAsync(defect.Id, tenantId, "Deleted", "Defect soft deleted");
        }

        public async Task<IList<DefectHistoryDto>> GetDefectHistoryAsync(
            Guid defectId,
            string tenantId
        )
        {
            var defect = await _context.Defects.FirstOrDefaultAsync(d =>
                d.Id == defectId && d.TenantId == tenantId && !d.IsDeleted
            );

            if (defect == null)
                throw new Exception("Defect not found.");

            var history = await _context
                .DefectHistories.Where(dh => dh.DefectId == defectId)
                .OrderBy(dh => dh.CreatedAt)
                .Select(dh => new DefectHistoryDto
                {
                    Id = dh.Id,
                    DefectId = dh.DefectId,
                    Action = dh.Action,
                    Details = dh.Details,
                    CreatedAt = dh.CreatedAt
                })
                .ToListAsync();

            return history;
        }

        private async Task LogDefectHistoryAsync(
            Guid defectId,
            string tenantId,
            string action,
            string details
        )
        {
            var history = new DefectHistory
            {
                Id = Guid.NewGuid(),
                DefectId = defectId,
                Action = action,
                //Details = details,
                CreatedAt = DateTime.UtcNow
            };

            await _context.DefectHistories.AddAsync(history);
            await _context.SaveChangesAsync();
        }
    }
}
