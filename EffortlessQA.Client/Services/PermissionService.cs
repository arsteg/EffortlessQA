namespace EffortlessQA.Client.Services
{
    public class PermissionService
    {
        private readonly HttpClient _httpClient;

        public PermissionService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("EffortlessQA.Api");
        }

        // Add methods for role/permission management as needed
    }
}
