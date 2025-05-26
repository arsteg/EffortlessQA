using EffortlessQA.Api.Services.Interface;
using EffortlessQA.Data;
using EffortlessQA.Data.Dtos;
using EffortlessQA.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EffortlessQA.Api.Services.Implementation
{
    public class MiscellaneousService : IMiscellaneousService
    {
        private readonly EffortlessQAContext _context;
        private readonly IConfiguration _configuration;

        public MiscellaneousService(EffortlessQAContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<List<CountryDto>> GetCountriesAsync()
        {
            var countries = await _context
                .Countries.OrderBy(c => c.Name)
                .Select(c => new CountryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Code = c.Code
                })
                .ToListAsync();

            return countries;
        }

        public async Task<AddressDto> CreateTenantAddressAsync(
            string tenantId,
            CreateAddressDto dto
        )
        {
            var tenant = await _context
                .Tenants.Include(t => t.Address)
                .FirstOrDefaultAsync(t => t.Id == tenantId && !t.IsDeleted);
            if (tenant == null)
                throw new Exception("Tenant not found.");

            if (tenant.Address != null && !tenant.Address.IsDeleted)
                throw new Exception("Tenant already has an address.");

            if (dto.CountryId.HasValue)
            {
                var country = await _context.Countries.FirstOrDefaultAsync(c =>
                    c.Id == dto.CountryId.Value
                );
                if (country == null)
                    throw new Exception("Country not found.");
            }

            var address = new Address
            {
                Id = Guid.NewGuid(),
                Address_Line1 = dto.AddressLine1,
                Address_Line2 = dto.AddressLine2,
                Address_Line3 = dto.AddressLine3,
                City = dto.City,
                State = dto.State,
                CountryId = dto.CountryId,
                Pincode = dto.Pincode,
                BillingContactEmail = dto.BillingContactEmail,
                TenantId = tenantId,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };

            tenant.Address = address;
            await _context.Addresses.AddAsync(address);
            await _context.SaveChangesAsync();

            return new AddressDto
            {
                Id = address.Id,
                AddressLine1 = address.Address_Line1,
                AddressLine2 = address.Address_Line2,
                AddressLine3 = address.Address_Line3,
                City = address.City,
                State = address.State,
                CountryId = address.CountryId,
                Pincode = address.Pincode,
                BillingContactEmail = address.BillingContactEmail,
                TenantId = address.TenantId,
                CreatedAt = address.CreatedAt,
                UpdatedAt = address.ModifiedAt
            };
        }

        public async Task<AddressDto> UpdateTenantAddressAsync(
            string tenantId,
            UpdateAddressDto dto
        )
        {
            var tenant = await _context
                .Tenants.Include(t => t.Address)
                .FirstOrDefaultAsync(t => t.Id == tenantId && !t.IsDeleted);
            if (tenant == null)
                throw new Exception("Tenant not found.");

            if (tenant.Address == null || tenant.Address.IsDeleted)
                throw new Exception("Tenant has no active address to update.");

            var address = tenant.Address;

            if (dto.CountryId.HasValue)
            {
                var country = await _context.Countries.FirstOrDefaultAsync(c =>
                    c.Id == dto.CountryId.Value
                );
                if (country == null)
                    throw new Exception("Country not found.");
                address.CountryId = dto.CountryId.Value;
            }

            address.Address_Line1 = dto.AddressLine1 ?? address.Address_Line1;
            address.Address_Line2 = dto.AddressLine2 ?? address.Address_Line2;
            address.Address_Line3 = dto.AddressLine3 ?? address.Address_Line3;
            address.City = dto.City ?? address.City;
            address.State = dto.State ?? address.State;
            address.Pincode = dto.Pincode ?? address.Pincode;
            address.BillingContactEmail = dto.BillingContactEmail ?? address.BillingContactEmail;
            address.ModifiedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new AddressDto
            {
                Id = address.Id,
                AddressLine1 = address.Address_Line1,
                AddressLine2 = address.Address_Line2,
                AddressLine3 = address.Address_Line3,
                City = address.City,
                State = address.State,
                CountryId = address.CountryId,
                Pincode = address.Pincode,
                BillingContactEmail = address.BillingContactEmail,
                TenantId = address.TenantId,
                CreatedAt = address.CreatedAt,
                UpdatedAt = address.ModifiedAt
            };
        }

        public async Task<SetupWizardDto> GetSetupWizardDataAsync(string tenantId)
        {
            var hasProject = await _context.Projects.AnyAsync(p =>
                p.TenantId == tenantId && !p.IsDeleted
            );

            var hasTestCase = await _context.TestCases.AnyAsync(tc =>
                tc.TenantId == tenantId && !tc.IsDeleted
            );

            var hasTestRun = await _context.TestRuns.AnyAsync(tr =>
                tr.TenantId == tenantId && !tr.IsDeleted
            );

            return new SetupWizardDto
            {
                HasProject = hasProject,
                HasTestCase = hasTestCase,
                HasTestRun = hasTestRun,
                NextStep = !hasProject
                    ? "Create a project"
                    : !hasTestCase
                        ? "Create a test case"
                        : !hasTestRun
                            ? "Create a test run"
                            : "Setup complete"
            };
        }
    }
}
