using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using EffortlessQA.Client.Models;

namespace EffortlessQA.Client.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage; // Add local storage service
        private bool _isAuthenticated;
        private bool _isAdmin;

        public AuthService(IHttpClientFactory httpClientFactory, ILocalStorageService localStorage)
        {
            _httpClient = httpClientFactory.CreateClient("EffortlessQA.Api");
            _localStorage = localStorage;
        }

        public bool IsAuthenticated => _isAuthenticated;
        public bool IsAdmin => _isAdmin;

        public async Task LoginAsync(LoginDto loginDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("Auth/login", loginDto);
                response.EnsureSuccessStatusCode();

                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
                if (loginResponse?.Data == null)
                {
                    throw new Exception("No token received from login response.");
                }

                await _localStorage.SetItemAsync("authToken", loginResponse.Data);

                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(loginResponse.Data);
                var claims = token.Claims.Select(c => $"{c.Type}: {c.Value}");
                Console.WriteLine("Token Claims: " + string.Join(", ", claims)); // Log claims

                var roleClaim = token.Claims.FirstOrDefault(c =>
                    c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
                );
                _isAdmin = roleClaim?.Value == "Admin";
                _isAuthenticated = true;
            }
            catch (Exception ex)
            {
                _isAuthenticated = false;
                _isAdmin = false;
                throw new Exception($"Login failed: {ex.Message}");
            }
        }

        public async Task RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("register", registerDto);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex) { }
        }

        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync("authToken");
            _isAuthenticated = false;
            _isAdmin = false;
        }

        public async Task<string> GetTokenAsync()
        {
            // Retrieve the token from local storage
            return await _localStorage.GetItemAsync<string>("authToken");
        }
    }
}
