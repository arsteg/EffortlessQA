using EffortlessQA.Data.Dtos;

namespace EffortlessQA.Api.Services.Interface
{
    public interface IMiscellaneousService
    {
        Task<List<CountryDto>> GetCountriesAsync();
        Task<AddressDto> CreateTenantAddressAsync(string tenantId, CreateAddressDto dto);
        Task<AddressDto> UpdateTenantAddressAsync(string tenantId, UpdateAddressDto dto);
        Task<SetupWizardDto> GetSetupWizardDataAsync(string tenantId);
    }
}
