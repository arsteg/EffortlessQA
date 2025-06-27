using System.Net.Http.Json;
using EffortlessQA.Data.Dtos;

namespace EffortlessQA.UI.Services
{
    public class DefectService
    {
        private readonly HttpClient _httpClient;

        public DefectService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("EffortlessQA.Api");
        }

        public async Task<List<DefectDto>> GetDefectsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<DefectDto>>("/defects") ?? new();
        }

        public async Task CreateDefectAsync(CreateDefectDto defectDto)
        {
            var response = await _httpClient.PostAsJsonAsync("/defects", defectDto);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteDefectAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"/defects/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
