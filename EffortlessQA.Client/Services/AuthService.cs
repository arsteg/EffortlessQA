using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using Blazored.LocalStorage;
using EffortlessQA.Data.Dtos;

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

        public async Task<string> GetCurrentTenantAsync()
        {
            try
            {
                // Example: Fetch tenant info from an API endpoint
                var response = await _httpClient.GetAsync("Auth/tenantCurrent");
                response.EnsureSuccessStatusCode();
                var tenant = await response.Content.ReadFromJsonAsync<ApiResponse<TenantDto>>();
                return tenant?.Data?.Name ?? "Unknown Tenant";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching tenant: {ex.Message}");
                return "Unknown Tenant";
            }
        }

        public async Task<string> GetTokenAsync()
        {
            // Retrieve the token from local storage
            return await _localStorage.GetItemAsync<string>("authToken");
        }

        public async Task InviteUserAsync(InviteUserDto inviteUserDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("users/invite", inviteUserDto);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex) { }
        }

        public async Task ChangePasswordAsync(
            Guid userId,
            string currentPassword,
            string newPassword
        )
        {
            try
            {
                var token = await _localStorage.GetItemAsync<string>("authToken");
                Console.WriteLine($"Token: {token}");

                var changePasswordDto = new ChangePasswordDto
                {
                    UserId = userId,
                    CurrentPassword = currentPassword,
                    NewPassword = newPassword
                };

                var response = await _httpClient.PostAsJsonAsync(
                    "auth/change-password",
                    changePasswordDto
                );
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception(
                    "Failed to change password. Please check your current password and try again.",
                    ex
                );
            }
            catch (Exception ex)
            {
                throw new Exception($"Error changing password: {ex.Message}", ex);
            }
        }

        // Add method to get user ID from JWT
        public async Task<Guid> GetUserIdAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrEmpty(token))
                throw new Exception("No authentication token found.");

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var userIdClaim = jwtToken.Claims.FirstOrDefault(c =>
                    c.Type == JwtRegisteredClaimNames.Sub
                    || c.Type == ClaimTypes.NameIdentifier
                    || c.Type == "name"
                );
                if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
                    throw new Exception("User ID not found in token.");

                if (!Guid.TryParse(userIdClaim.Value, out var userId))
                    throw new Exception("Invalid user ID format in token.");

                return userId;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error reading user ID from token: {ex.Message}", ex);
            }
        }
    }
}
