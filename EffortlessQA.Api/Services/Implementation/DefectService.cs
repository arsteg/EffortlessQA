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

        public DefectService(EffortlessQAContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<DefectDto>> GetDefectsAsync(
            string tenantId,
            int page,
            int limit,
            string sort,
            string filter
        )
        {
            var query = _context
                .Defects.AsNoTracking()
                .Where(d => d.TenantId == tenantId && !d.IsDeleted);

            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(d =>
                    d.Title.Contains(filter) || d.Description.Contains(filter)
                );
            }

            //query = query.OrderBy(sort);
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(d => new DefectDto
                {
                    Id = d.Id,
                    Title = d.Title,
                    Description = d.Description,
                    Severity = d.Severity,
                    Status = d.Status,
                    Attachments = d.Attachments,
                    ExternalId = d.ExternalId,
                    TestRunResultId = d.TestRunResultId,
                    TestCaseId = d.TestCaseId,
                    AssignedUserId = d.AssignedUserId,
                    TenantId = d.TenantId
                })
                .ToListAsync();

            return new PagedResult<DefectDto> { Items = items, TotalCount = totalCount };
        }

        public async Task<DefectDto> CreateDefectAsync(DefectCreateDto dto, string tenantId)
        {
            var defect = new Defect
            {
                Title = dto.Title,
                Description = dto.Description,
                Severity = dto.Severity,
                Status = DefectStatus.Open,
                Attachments = (System.Text.Json.JsonDocument)dto.Attachments,
                ExternalId = dto.ExternalId,
                TestRunResultId = dto.TestRunResultId,
                TestCaseId = dto.TestCaseId,
                AssignedUserId = dto.AssignedUserId,
                TenantId = tenantId
            };

            _context.Defects.Add(defect);
            await _context.SaveChangesAsync();

            return new DefectDto
            {
                Id = defect.Id,
                Title = defect.Title,
                Description = defect.Description,
                Severity = defect.Severity,
                Status = defect.Status,
                Attachments = defect.Attachments,
                ExternalId = defect.ExternalId,
                TestRunResultId = defect.TestRunResultId,
                TestCaseId = defect.TestCaseId,
                AssignedUserId = defect.AssignedUserId,
                TenantId = defect.TenantId
            };
        }
    }
}
