using EffortlessQA.Api.Services.Interface;
using EffortlessQA.Data;
using EffortlessQA.Data.Dtos;
using EffortlessQA.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EffortlessQA.Api.Services.Implementation
{
    public class TenantService : ITenantService
    {
        private readonly EffortlessQAContext _context;
        private readonly IConfiguration _configuration;

        public TenantService(EffortlessQAContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<TenantDto> CreateTenantAsync(CreateTenantDto dto)
        {
            var tenant = new Tenant
            {
                Id = Guid.NewGuid().ToString(),
                Name = dto.Name,
                ContactPerson = dto.ContactPerson,
                Email = dto.Email,
                Phone = dto.Phone,
                Description = dto.Description,

                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };

            await _context.Tenants.AddAsync(tenant);
            await _context.SaveChangesAsync();

            return new TenantDto
            {
                Id = tenant.Id,
                Name = tenant.Name,
                ContactPerson = tenant.ContactPerson,
                Email = tenant.Email,
                Phone = tenant.Phone,
                Description = tenant.Description,
                BillingContactEmail = tenant.BillingContactEmail
            };
        }

        public async Task<PagedResult<TenantDto>> GetTenantsAsync(
            int page,
            int limit,
            string? sort,
            string? filter
        )
        {
            var query = _context.Tenants.Where(t => !t.IsDeleted);

            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(t => t.Name.Contains(filter) || t.Email.Contains(filter));
            }

            if (!string.IsNullOrEmpty(sort))
            {
                query = sort.ToLower() switch
                {
                    "name" => query.OrderBy(t => t.Name),
                    "-name" => query.OrderByDescending(t => t.Name),
                    "email" => query.OrderBy(t => t.Email),
                    "-email" => query.OrderByDescending(t => t.Email),
                    _ => query.OrderBy(t => t.Name)
                };
            }

            var totalCount = await query.CountAsync();
            var tenants = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(t => new TenantDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    ContactPerson = t.ContactPerson,
                    Email = t.Email,
                    Phone = t.Phone,
                    Description = t.Description,
                    BillingContactEmail = t.BillingContactEmail
                })
                .ToListAsync();

            return new PagedResult<TenantDto>
            {
                Items = tenants,
                TotalCount = totalCount,
                Page = page,
                Limit = limit
            };
        }

        public async Task<TenantDto> GetTenantAsync(string tenantId)
        {
            var tenant = await _context.Tenants.FirstOrDefaultAsync(t =>
                t.Id == tenantId && !t.IsDeleted
            );

            if (tenant == null)
                throw new Exception("Tenant not found.");

            return new TenantDto
            {
                Id = tenant.Id,
                Name = tenant.Name,
                ContactPerson = tenant.ContactPerson,
                Email = tenant.Email,
                Phone = tenant.Phone,
                Description = tenant.Description,
                BillingContactEmail = tenant.BillingContactEmail
            };
        }

        public async Task<TenantDto> UpdateTenantAsync(string tenantId, UpdateTenantDto dto)
        {
            var tenant = await _context.Tenants.FirstOrDefaultAsync(t =>
                t.Id == tenantId && !t.IsDeleted
            );

            if (tenant == null)
                throw new Exception("Tenant not found.");

            tenant.Name = dto.Name ?? tenant.Name;
            tenant.ContactPerson = dto.ContactPerson ?? tenant.ContactPerson;
            tenant.Email = dto.Email ?? tenant.Email;
            tenant.Phone = dto.Phone ?? tenant.Phone;
            tenant.Description = dto.Description ?? tenant.Description;

            tenant.ModifiedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new TenantDto
            {
                Id = tenant.Id,
                Name = tenant.Name,
                ContactPerson = tenant.ContactPerson,
                Email = tenant.Email,
                Phone = tenant.Phone,
                Description = tenant.Description,
                BillingContactEmail = tenant.BillingContactEmail
            };
        }

        public async Task DeleteTenantAsync(string tenantId)
        {
            var tenant = await _context.Tenants.FirstOrDefaultAsync(t =>
                t.Id == tenantId && !t.IsDeleted
            );

            if (tenant == null)
                throw new Exception("Tenant not found.");

            tenant.IsDeleted = true;
            tenant.ModifiedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
