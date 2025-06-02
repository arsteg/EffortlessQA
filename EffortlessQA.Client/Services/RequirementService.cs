using System.Net.Http.Json;

using EffortlessQA.Data.Dtos;

namespace EffortlessQA.Client.Services
{
    public class RequirementService
    {
        private readonly HttpClient _httpClient;

        public RequirementService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("EffortlessQA.Api");
        }

        public async Task<List<RequirementDto>> GetRequirementsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<RequirementDto>>("/requirements")
                ?? new();
        }

        public async Task CreateRequirementAsync(CreateRequirementDto requirementDto)
        {
            var response = await _httpClient.PostAsJsonAsync("/requirements", requirementDto);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteRequirementAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"/requirements/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
