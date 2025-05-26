using EffortlessQA.Data.Dtos;

namespace EffortlessQA.Api.Services.Interface
{
    public interface ITenantService
    {
        Task<TenantDto> CreateTenantAsync(CreateTenantDto dto);
        Task<PagedResult<TenantDto>> GetTenantsAsync(
            int page,
            int limit,
            string? sort,
            string? filter
        );
        Task<TenantDto> GetTenantAsync(string tenantId);
        Task<TenantDto> UpdateTenantAsync(string tenantId, UpdateTenantDto dto);
        Task DeleteTenantAsync(string tenantId);
    }
}
