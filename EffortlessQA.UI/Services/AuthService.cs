using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using EffortlessQA.Data.Dtos;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage; // For CSRF token storage if needed
using Microsoft.AspNetCore.Http;

namespace EffortlessQA.UI.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAntiforgery _antiforgery;
        private bool _isAuthenticated;
        private bool _isAdmin;

        public AuthService(
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            IAntiforgery antiforgery
        )
        {
            _httpClient = httpClientFactory.CreateClient("EffortlessQAApi");
            _httpContextAccessor = httpContextAccessor;
            _antiforgery = antiforgery;
        }

        public bool IsAuthenticated => _isAuthenticated;
        public bool IsAdmin => _isAdmin;

        public async Task LoginAsync(LoginDto loginDto)
        {
            try
            {
                // Get CSRF token
                //var antiforgeryToken = await GetCsrfTokenAsync();

                // Add CSRF token to headers
                //_httpClient.DefaultRequestHeaders.Add("X-CSRF-TOKEN", antiforgeryToken);

                var response = await _httpClient.PostAsJsonAsync("auth/login", loginDto);
                response.EnsureSuccessStatusCode();

                var loginResponse = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
                if (loginResponse?.Data == null)
                {
                    throw new Exception("No token received from login response.");
                }

                // Parse JWT to extract claims (optional, for role checking)
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(loginResponse.Data);
                var claims = token.Claims.Select(c => $"{c.Type}: {c.Value}");
                Console.WriteLine("Token Claims: " + string.Join(", ", claims)); // Log claims

                var roleClaim = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
                _isAdmin = roleClaim?.Value == "Admin";
                _isAuthenticated = true;

                // Clear CSRF token header after request
                _httpClient.DefaultRequestHeaders.Remove("X-CSRF-TOKEN");
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
                // Get CSRF token
                var antiforgeryToken = await GetCsrfTokenAsync();
                _httpClient.DefaultRequestHeaders.Add("X-CSRF-TOKEN", antiforgeryToken);

                var response = await _httpClient.PostAsJsonAsync("api/auth/register", registerDto);
                response.EnsureSuccessStatusCode();

                // Clear CSRF token header
                _httpClient.DefaultRequestHeaders.Remove("X-CSRF-TOKEN");
            }
            catch (Exception ex)
            {
                throw new Exception($"Registration failed: {ex.Message}");
            }
        }

        public async Task LogoutAsync()
        {
            try
            {
                // Call logout endpoint to clear cookies
                var response = await _httpClient.PostAsync("api/auth/logout", null);
                response.EnsureSuccessStatusCode();
                _isAuthenticated = false;
                _isAdmin = false;
            }
            catch (Exception ex)
            {
                throw new Exception($"Logout failed: {ex.Message}");
            }
        }

        public async Task<string> GetCurrentTenantAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("auth/tenantCurrent");
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

        public async Task InviteUserAsync(InviteUserDto inviteUserDto)
        {
            try
            {
                // Get CSRF token
                var antiforgeryToken = await GetCsrfTokenAsync();
                _httpClient.DefaultRequestHeaders.Add("X-CSRF-TOKEN", antiforgeryToken);

                var response = await _httpClient.PostAsJsonAsync("api/users/invite", inviteUserDto);
                response.EnsureSuccessStatusCode();

                // Clear CSRF token header
                _httpClient.DefaultRequestHeaders.Remove("X-CSRF-TOKEN");
            }
            catch (Exception ex)
            {
                throw new Exception($"Invite user failed: {ex.Message}");
            }
        }

        public async Task ChangePasswordAsync(
            Guid userId,
            string currentPassword,
            string newPassword
        )
        {
            try
            {
                // Get CSRF token
                var antiforgeryToken = await GetCsrfTokenAsync();
                _httpClient.DefaultRequestHeaders.Add("X-CSRF-TOKEN", antiforgeryToken);

                var changePasswordDto = new ChangePasswordDto
                {
                    UserId = userId,
                    CurrentPassword = currentPassword,
                    NewPassword = newPassword
                };

                var response = await _httpClient.PostAsJsonAsync(
                    "api/auth/change-password",
                    changePasswordDto
                );
                response.EnsureSuccessStatusCode();

                // Clear CSRF token header
                _httpClient.DefaultRequestHeaders.Remove("X-CSRF-TOKEN");
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

        public async Task<Guid> GetUserIdAsync()
        {
            try
            {
                // Call an endpoint to get the current user's ID (requires authentication)
                var response = await _httpClient.GetAsync("api/auth/user-profile");
                response.EnsureSuccessStatusCode();
                var userProfile = await response.Content.ReadFromJsonAsync<ApiResponse<UserDto>>();
                if (userProfile?.Data == null)
                    throw new Exception("User profile not found.");

                return userProfile.Data.Id;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving user ID: {ex.Message}", ex);
            }
        }

        private async Task<string> GetCsrfTokenAsync()
        {
            // Get CSRF token from the antiforgery service
            var tokens = _antiforgery.GetAndStoreTokens(_httpContextAccessor.HttpContext);
            return tokens.RequestToken;
        }
    }
}
