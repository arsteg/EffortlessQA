using System.Net.Http.Json;
using EffortlessQA.Client.Models;

namespace EffortlessQA.Client.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private bool _isAuthenticated;
        private bool _isAdmin;

        public AuthService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("EffortlessQA.Api");
        }

        public bool IsAuthenticated => _isAuthenticated;
        public bool IsAdmin => _isAdmin;

        public async Task LoginAsync(LoginDto loginDto)
        {
            var response = await _httpClient.PostAsJsonAsync("/auth/login", loginDto);
            response.EnsureSuccessStatusCode();
            var user = await response.Content.ReadFromJsonAsync<UserDto>();
            _isAuthenticated = true;
            _isAdmin = true; // Simplified; check roles from JWT in real implementation
        }

        public async Task RegisterAsync(RegisterDto registerDto)
        {
            var response = await _httpClient.PostAsJsonAsync("/auth/register", registerDto);
            response.EnsureSuccessStatusCode();
        }

        public async Task LogoutAsync()
        {
            _isAuthenticated = false;
            _isAdmin = false;
        }
    }
}
