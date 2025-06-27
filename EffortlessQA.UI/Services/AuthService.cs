using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using EffortlessQA.Data.Dtos;
using Microsoft.AspNetCore.Http;
using Blazored.LocalStorage; // Use Blazored.LocalStorage instead of ProtectedSessionStorage
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace EffortlessQA.UI.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly ILocalStorageService _localStorage; // Changed to ILocalStorageService
		private bool _isAuthenticated;
        private bool _isAdmin;

        public AuthService(
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor,
			ILocalStorageService localStorage
		)
        {
            _httpClient = httpClientFactory.CreateClient("EffortlessQAApi");
            _httpContextAccessor = httpContextAccessor;
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

				// Store token in session storage
				//_httpContextAccessor.HttpContext.Session.SetString("authToken", loginResponse.Data);

				//await _localStorage.SetItemAsync("authToken",loginResponse.Data);

				var handler = new JwtSecurityTokenHandler();
				var token = handler.ReadJwtToken(loginResponse.Data);
				var claims = token.Claims.Select(c => $"{c.Type}: {c.Value}");
				Console.WriteLine("Token Claims: " + string.Join(", ",claims)); // Log claims

				var roleClaim = token.Claims.FirstOrDefault(c =>
					c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
				);
				_isAdmin = roleClaim?.Value == "Admin";

				_isAuthenticated = true;
                // TODO: Implement IsAdmin logic based on token claims if needed
                // Example: _isAdmin = await CheckAdminRoleAsync(loginResponse.Data);
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
            catch (Exception ex)
            {
                throw new Exception($"Registration failed: {ex.Message}");
            }
        }

        public async Task LogoutAsync()
        {
            //_httpContextAccessor.HttpContext.Session.Remove("authToken");
			await _localStorage.RemoveItemAsync("authToken"); // Changed to RemoveItemAsync
			_isAuthenticated = false;
            _isAdmin = false;
            await Task.CompletedTask;
        }

        public async Task<string> GetCurrentTenantAsync()
        {
            try
            {
				var token = await GetTokenAsync();
				if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

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
            //return _httpContextAccessor.HttpContext.Session.GetString("authToken");
            //return await _localStorage.GetItemAsync<string>("authToken");
            return "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJlMDlhZDc5MS00MGQxLTQ5MzctODI5Yi1lNGM5Y2Q3ZTYyN2YiLCJlbWFpbCI6Im1vaGRyYWZpb25saW5lQGdtYWlsLmNvbSIsInRlbmFudElkIjoiNTUwYjA2ZjQ3ZDI4NDkxMGJhM2MyNzE1MGU1MmFhMTgiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJBZG1pbiIsImV4cCI6MTc4MjU2Mjg4OSwiaXNzIjoiRWZmb3J0bGVzc1FBIiwiYXVkIjoiRWZmb3J0bGVzc1FBVXNlcnMifQ.gEoALuWkuf_-UT7cCy0ZfgeYpbSWWUI0ThuCgqRQ2uk";
		}

        public async Task InviteUserAsync(InviteUserDto inviteUserDto)
        {
            try
            {
                var token = await GetTokenAsync();
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var response = await _httpClient.PostAsJsonAsync("users/invite", inviteUserDto);
                response.EnsureSuccessStatusCode();
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
                var token = await GetTokenAsync();
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

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

        public async Task<Guid> GetUserIdAsync()
        {
            var token = await GetTokenAsync();
            if (string.IsNullOrEmpty(token))
                throw new Exception("No authentication token found.");

            try
            {
                // TODO: Implement JWT parsing if needed
                // Example:
                // var handler = new JwtSecurityTokenHandler();
                // var jwtToken = handler.ReadJwtToken(token);
                // var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub" || c.Type == ClaimTypes.NameIdentifier);
                // if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                //     throw new Exception("Invalid user ID format in token.");
                // return userId;

                // Placeholder: Replace with actual logic
                return Guid.NewGuid();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error reading user ID from token: {ex.Message}", ex);
            }
        }
    }
}
