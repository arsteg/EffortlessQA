using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using EffortlessQA.Api.Services.Interface;
using EffortlessQA.Data;
using EffortlessQA.Data.Dtos;
using EffortlessQA.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace EffortlessQA.Api.Services.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly IEmailService _emailService;
        private readonly EffortlessQAContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(
            EffortlessQAContext context,
            IConfiguration configuration,
            IEmailService emailService,
            IHttpContextAccessor httpContextAccessor
        )
        {
            //_userManager = userManager;
            // _signInManager = signInManager;
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<UserDto> RegisterAsync(RegisterDto dto)
        {
            // Validate input
            if (
                string.IsNullOrWhiteSpace(dto.Email)
                || string.IsNullOrWhiteSpace(dto.Password)
                || string.IsNullOrWhiteSpace(dto.FirstName)
                || dto.Tenant == null
            )
                throw new Exception("Email, password, name, and tenant details are required.");

            // Check if tenant email already exists
            var existingTenant = await _context.Tenants.FirstOrDefaultAsync(t =>
                t.Email == dto.Tenant.Email && !t.IsDeleted
            );
            if (existingTenant != null)
                throw new Exception("A company with this email already exists.");

            // Check if user email already exists
            var existingUser = await _context.Users.FirstOrDefaultAsync(u =>
                u.Email == dto.Email && !u.IsDeleted
            );
            if (existingUser != null)
                throw new Exception("A user with this email already exists.");

            // Generate unique TenantId
            var tenantId = Guid.NewGuid().ToString("N");

            // Begin transaction
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Create Tenant
                var tenant = new Tenant
                {
                    Id = tenantId,
                    Name = dto.Tenant.Name,
                    ContactPerson = dto.Tenant.ContactPerson,
                    Email = dto.Tenant.Email,
                    Phone = dto.Tenant.Phone,
                    BillingContactEmail = dto.Tenant.BillingContactEmail ?? dto.Tenant.Email,
                    IsEmailConfirmed = false,
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
                    CreatedBy = Guid.Empty,
                    ModifiedBy = Guid.Empty
                };

                await _context.Tenants.AddAsync(tenant);

                // Create User
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = dto.Email,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    TenantId = tenantId,
                    IsEmailConfirmed = false,
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
                    CreatedBy = Guid.Empty,
                    ModifiedBy = Guid.Empty
                };

                await _context.Users.AddAsync(user);

                // Assign Admin role
                var role = new Role
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    //ProjectId = Guid.Empty, // No project yet
                    RoleType = RoleType.Admin,
                    TenantId = tenantId,
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
                    CreatedBy = user.Id,
                    ModifiedBy = user.Id
                };
                await _context.Roles.AddAsync(role);

                // Generate email confirmation tokens
                var userToken = Guid.NewGuid().ToString();
                var userConfirmation = new UserEmailConfirmation
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Token = userToken,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddHours(24)
                };
                await _context.UserEmailConfirmations.AddAsync(userConfirmation);

                var tenantToken = Guid.NewGuid().ToString();
                var tenantConfirmation = new TenantEmailConfirmation
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    Token = tenantToken,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddHours(24)
                };
                await _context.TenantEmailConfirmations.AddAsync(tenantConfirmation);

                // Generate confirmation links
                var baseUrl =
                    _configuration["AppSettings:BaseUrl"]
                    ?? throw new Exception("BaseUrl is not configured.");
                var userConfirmationLink =
                    $"{baseUrl}/api/auth/confirm-user-email?userId={user.Id}&token={Uri.EscapeDataString(userToken)}";
                var tenantConfirmationLink =
                    $"{baseUrl}/api/auth/confirm-tenant-email?tenantId={tenantId}&token={Uri.EscapeDataString(tenantToken)}";

                // Send confirmation emails
                await _emailService.SendRegistrationConfirmationAsync(
                    dto.Email,
                    dto.FirstName,
                    userConfirmationLink
                );
                await _emailService.SendTenantConfirmationAsync(
                    dto.Tenant.Email,
                    dto.Tenant.Name,
                    tenantConfirmationLink
                );

                // Save changes
                await _context.SaveChangesAsync();

                // Log audit entries
                var tenantAuditLog = new AuditLog
                {
                    Id = Guid.NewGuid(),
                    Action = "TenantCreated",
                    EntityType = "Tenant",
                    EntityId = Guid.Parse(tenantId),
                    UserId = user.Id,
                    TenantId = tenantId,
                    Details = JsonDocument.Parse(
                        JsonSerializer.Serialize(new { Name = tenant.Name, Email = tenant.Email })
                    ),
                    CreatedAt = DateTime.UtcNow
                };
                await _context.AuditLogs.AddAsync(tenantAuditLog);

                var userAuditLog = new AuditLog
                {
                    Id = Guid.NewGuid(),
                    Action = "UserCreated",
                    EntityType = "User",
                    EntityId = user.Id,
                    UserId = user.Id,
                    //ProjectId = Guid.Empty,
                    TenantId = tenantId,
                    Details = JsonDocument.Parse(
                        JsonSerializer.Serialize(new { Name = tenant.Name, Email = tenant.Email })
                    ),
                    CreatedAt = DateTime.UtcNow
                };
                await _context.AuditLogs.AddAsync(userAuditLog);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    TenantId = user.TenantId
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<string> LoginAsync(LoginDto dto)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                throw new ArgumentException("Email and password are required.");

            // Find user
            var user = await _context
                .Users.IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Email == dto.Email && !u.IsDeleted);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid email or password.");

            // Generate JWT token
            var token = GenerateJwtToken(user);

            // Set cookies before any response is written
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(1)
            };

            _httpContextAccessor?.HttpContext?.Response.Cookies.Append(
                "access_token",
                token,
                cookieOptions
            );
            _httpContextAccessor?.HttpContext?.Response.Cookies.Append(
                "TenantId",
                user.TenantId,
                cookieOptions
            );

            return token;
        }

        public async Task<string> OAuthLoginAsync(OAuthLoginDto dto, string provider)
        {
            // Placeholder: Validate OAuth token with provider (Google/GitHub)
            var userInfo = await ValidateOAuthTokenAsync(dto.Token, provider);
            var user = await _context.Users.FindAsync(userInfo.Email);

            if (user == null)
            {
                user = new User
                {
                    Email = userInfo.Email,
                    FirstName = userInfo.Name,
                    TenantId = dto.TenantId,
                    OAuthProvider = provider,
                    OAuthId = userInfo.OAuthId
                };
                var result = await _context.Users.AddAsync(user);
                //if (!result.Succeeded)
                //    throw new Exception(
                //        string.Join(", ", result.Errors.Select(e => e.Description))
                //    );

                //await _userManager.AddToRoleAsync(user, RoleType.Tester.ToString());
            }

            return GenerateJwtToken(user);
        }

        public async Task<UserDto> GetUserProfileAsync(Guid userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Id == userId && !u.IsDeleted
            );
            if (user == null)
                throw new Exception("User not found.");

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                TenantId = user.TenantId
            };
        }

        public async Task<UserDto> UpdateUserProfileAsync(Guid userId, UpdateUserDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Id == userId && !u.IsDeleted
            );
            if (user == null)
                throw new Exception("User not found.");

            user.FirstName = dto.Name ?? user.FirstName;
            user.Email = dto.Email ?? user.Email;
            user.ModifiedAt = DateTime.UtcNow;
            user.ModifiedBy = userId;

            await _context.SaveChangesAsync();

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                TenantId = user.TenantId
            };
        }

        public async Task RequestPasswordResetAsync(
            PasswordResetRequestDto dto,
            IEmailService emailService
        )
        {
            var user = await _context.Users.FindAsync(dto.Email);
            if (user == null)
                return; // Silent fail for security

            var token = ""; // await _context.Users.GeneratePasswordResetTokenAsync(user);
            var resetLink =
                $"https://effortlessqa.com/reset-password?email={dto.Email}&token={Uri.EscapeDataString(token)}";

            await emailService.SendEmailAsync(
                dto.Email,
                "Password Reset Request",
                $"Click here to reset your password: {resetLink}"
            );
        }

        public async Task ConfirmPasswordResetAsync(PasswordResetConfirmDto dto)
        {
            var user = await _context.Users.FindAsync(dto.Email);
            if (user == null)
                throw new Exception("Invalid email.");

            //var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
            //if (!result.Succeeded)
            //    throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        public async Task<TenantDto> GetCurrentTenantAsync()
        {
            try
            {
                // Retrieve tenantId from JWT claims
                var tenantId = _httpContextAccessor.HttpContext?.User.FindFirst("TenantId")?.Value;

                if (string.IsNullOrEmpty(tenantId))
                {
                    throw new Exception("TenantId not found in user claims.");
                }

                // Query the Tenants table to get the tenant details
                var tenant = await _context
                    .Tenants.Where(t => t.Id == tenantId && !t.IsDeleted)
                    .Select(t => new TenantDto { Id = t.Id, Name = t.Name })
                    .FirstOrDefaultAsync();

                if (tenant == null)
                {
                    throw new Exception("Tenant not found.");
                }

                return tenant;
            }
            catch (Exception ex)
            {
                Log.Error("Error fetching tenant: {Message}", ex.Message);
                return new TenantDto { Name = "Unknown Tenant" }; // Fallback
            }
        }

        public async Task<UserDto> InviteUserAsync(
            InviteUserDto dto,
            string tenantId,
            IEmailService emailService
        )
        {
            // Validate inputs
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));
            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new ArgumentException("Email is required.", nameof(dto.Email));
            if (string.IsNullOrWhiteSpace(dto.FirstName))
                throw new ArgumentException("Name is required.", nameof(dto.FirstName));
            if (string.IsNullOrWhiteSpace(tenantId))
                throw new ArgumentException("Tenant ID is required.", nameof(tenantId));
            if (emailService == null)
                throw new ArgumentNullException(nameof(emailService));

            // Start a database transaction
            using var transaction = await _context.Database.BeginTransactionAsync();
            var tempPassword = string.Empty;
            try
            {
                // Check for existing user
                var existingUser = await _context.Users.FirstOrDefaultAsync(u =>
                    u.Email == dto.Email && u.TenantId == tenantId
                );

                User user;
                bool isNewUser = existingUser == null;

                if (isNewUser)
                {
                    // Create new user
                    tempPassword = GenerateTempPassword();
                    user = new User
                    {
                        Id = Guid.NewGuid(),
                        Email = dto.Email,
                        FirstName = dto.FirstName,
                        LastName = dto.LastName,
                        TenantId = tenantId,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword(tempPassword),
                    };

                    await _context.Users.AddAsync(user);
                }
                else
                {
                    user = existingUser;
                }

                // Validate project
                var project = await _context.Projects.FirstOrDefaultAsync(p =>
                    p.Id == dto.ProjectId && p.TenantId == tenantId && !p.IsDeleted
                );
                if (project == null)
                    throw new InvalidOperationException("Project not found or is deleted.");

                // Check if user already has a role in this project
                var existingRole = await _context.Roles.FirstOrDefaultAsync(r =>
                    r.UserId == user.Id && r.TenantId == tenantId
                );
                if (existingRole != null)
                    throw new InvalidOperationException(
                        $"User {dto.Email} already has a role in project {project.Name}."
                    );

                // Assign role
                var role = new Role
                {
                    UserId = user.Id,
                    RoleType = dto.RoleType,
                    TenantId = tenantId
                };
                await _context.Roles.AddAsync(role);

                // Save changes to database
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    throw new InvalidOperationException(
                        $"Failed to save user or role: {ex.InnerException?.Message ?? ex.Message}"
                    );
                }

                // Send email for new users
                if (isNewUser)
                {
                    await emailService.SendEmailAsync(
                        dto.Email,
                        "EffortlessQA Invitation",
                        $"You have been invited to join EffortlessQA. Your temporary password is: {tempPassword}"
                    );
                }

                // Commit transaction
                await transaction.CommitAsync();

                // Return UserDto
                return new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    TenantId = user.TenantId,
                };
            }
            catch (Exception)
            {
                // Roll back transaction on error
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<PagedResult<UserDto>> GetUsersAsync(
            string tenantId,
            int page,
            int limit,
            string? sort,
            string? filter
        )
        {
            var query = _context.Users.Where(u => u.TenantId == tenantId && !u.IsDeleted);

            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(u => u.FirstName.Contains(filter) || u.Email.Contains(filter));
            }

            if (!string.IsNullOrEmpty(sort))
            {
                query = sort.ToLower() switch
                {
                    "name" => query.OrderBy(u => u.FirstName),
                    "-name" => query.OrderByDescending(u => u.FirstName),
                    "email" => query.OrderBy(u => u.Email),
                    "-email" => query.OrderByDescending(u => u.Email),
                    _ => query.OrderBy(u => u.FirstName)
                };
            }

            var totalCount = await query.CountAsync();
            var users = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    TenantId = u.TenantId
                })
                .ToListAsync();

            return new PagedResult<UserDto>
            {
                Items = users,
                TotalCount = totalCount,
                Page = page,
                Limit = limit
            };
        }

        public async Task DeleteUserAsync(Guid userId, string tenantId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Id == userId && u.TenantId == tenantId && !u.IsDeleted
            );
            if (user == null)
                throw new Exception("User not found.");

            user.IsDeleted = true;
            user.ModifiedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task ChangePasswordAsync(Guid userId, ChangePasswordDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));
            if (string.IsNullOrWhiteSpace(dto.CurrentPassword))
                throw new ArgumentException(
                    "Current password is required.",
                    nameof(dto.CurrentPassword)
                );
            if (string.IsNullOrWhiteSpace(dto.NewPassword))
                throw new ArgumentException("New password is required.", nameof(dto.NewPassword));

            // Find the user
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Id == userId && !u.IsDeleted
            );
            if (user == null)
                throw new Exception("User not found.");

            // Verify current password
            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
                throw new Exception("Current password is incorrect.");

            // Optional: Enforce password complexity
            if (dto.NewPassword.Length < 8)
                throw new Exception("New password must be at least 8 characters long.");
            if (!dto.NewPassword.Any(char.IsUpper))
                throw new Exception("New password must contain at least one uppercase letter.");
            if (!dto.NewPassword.Any(char.IsDigit))
                throw new Exception("New password must contain at least one number.");

            // Update password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.ModifiedAt = DateTime.UtcNow;
            user.ModifiedBy = userId;

            // Log audit entry
            var auditLog = new AuditLog
            {
                Id = Guid.NewGuid(),
                Action = "PasswordChanged",
                EntityType = "User",
                EntityId = userId,
                UserId = userId,
                TenantId = user.TenantId,
                Details = JsonDocument.Parse(
                    JsonSerializer.Serialize(new { Message = "User changed their password" })
                ),
                CreatedAt = DateTime.UtcNow
            };
            await _context.AuditLogs.AddAsync(auditLog);

            // Save changes
            await _context.SaveChangesAsync();
        }

        private async Task<(string Email, string Name, string OAuthId)> ValidateOAuthTokenAsync(
            string token,
            string provider
        )
        {
            // Placeholder: Implement OAuth token validation (Google/GitHub)
            // Example for Google (requires Google.Apis.Auth):
            /*
            var payload = await GoogleJsonWebSignature.ValidateAsync(token);
            return (payload.Email, payload.Name, payload.Subject);
            */
            throw new NotImplementedException($"OAuth validation for {provider} not implemented.");
        }

        private string GenerateJwtToken(User user)
        {
            var keyString = _configuration["JwtSettings:SecretKey"];
            if (string.IsNullOrEmpty(keyString))
            {
                Log.Error("JWT key is null or empty in configuration.");
                throw new InvalidOperationException("JWT key is null or empty in configuration.");
            }

            var keyBytes = Encoding.UTF8.GetBytes(keyString);
            if (keyBytes.Length < 32)
            {
                Log.Error(
                    "JWT key is too short: {KeyLength} bytes, expected at least 32 bytes.",
                    keyBytes.Length
                );
                throw new InvalidOperationException(
                    $"JWT key is too short: {keyBytes.Length} bytes, expected at least 32 bytes."
                );
            }

            Log.Information("JWT key length: {KeyLength} bytes", keyBytes.Length);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("TenantId", user.TenantId),
                new Claim(
                    ClaimTypes.Role,
                    user.Roles.FirstOrDefault()?.RoleType.ToString() ?? RoleType.Admin.ToString()
                )
            };

            var key = new SymmetricSecurityKey(keyBytes);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiresInMinutes = _configuration.GetValue<int>("JwtSettings:ExpiresInMinutes", 60);
            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateTempPassword()
        {
            // Simple temp password generator
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(
                Enumerable.Repeat(chars, 12).Select(s => s[random.Next(s.Length)]).ToArray()
            );
        }
    }
}
