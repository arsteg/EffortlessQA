using System.Net.Http.Json;
using EffortlessQA.Data.Dtos;

namespace EffortlessQA.UI.Services
{
    public class TenantService
    {
        private readonly HttpClient _httpClient;

        public TenantService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("EffortlessQA.Api");
        }

        public async Task CreateTenantAsync(CreateTenantDto tenantDto)
        {
            var response = await _httpClient.PostAsJsonAsync("/tenants", tenantDto);
            response.EnsureSuccessStatusCode();
        }

        public async Task RegisterUserAsync(RegisterDto registerDto)
        {
            var response = await _httpClient.PostAsJsonAsync("/auth/register", registerDto);
            response.EnsureSuccessStatusCode();
        }
    }
}
