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

namespace EffortlessQA.Api.Services.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly IEmailService _emailService;
        private readonly EffortlessQAContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(
            EffortlessQAContext context,
            IConfiguration configuration,
            IEmailService emailService
        )
        {
            //_userManager = userManager;
            // _signInManager = signInManager;
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<UserDto> RegisterAsync(RegisterDto dto)
        {
            // Validate input
            if (
                string.IsNullOrWhiteSpace(dto.Email)
                || string.IsNullOrWhiteSpace(dto.Password)
                || string.IsNullOrWhiteSpace(dto.Name)
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
                    Name = dto.Name,
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
                    dto.Name,
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
                        JsonSerializer.Serialize(new { Email = user.Email, Name = user.Name })
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
                    Name = user.Name,
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
            var users = await _context.Users.IgnoreQueryFilters().ToListAsync();

            if (users == null || !users.Any())
                throw new Exception("No users found.");

            var user = await _context.Users.FirstOrDefaultAsync(u => true);
            if (user == null)
                throw new Exception("Invalid email or password.");

            //var result = await _signInManager.PasswordSignInAsync(user, dto.Password, false, false);
            //if (!result.Succeeded)
            //    throw new Exception("Invalid email or password.");

            return GenerateJwtToken(user);
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
                    Name = userInfo.Name,
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
                Name = user.Name,
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

            user.Name = dto.Name ?? user.Name;
            user.Email = dto.Email ?? user.Email;
            user.ModifiedAt = DateTime.UtcNow;
            user.ModifiedBy = userId;

            await _context.SaveChangesAsync();

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
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

        public async Task<UserDto> InviteUserAsync(
            InviteUserDto dto,
            string tenantId,
            IEmailService emailService
        )
        {
            var existingUser = await _context.Users.FindAsync(dto.Email);
            var user = existingUser;
            var isNewUser = user == null;

            if (isNewUser)
            {
                user = new User
                {
                    Email = dto.Email,
                    Name = dto.Name,
                    TenantId = tenantId
                };
                var tempPassword = GenerateTempPassword();
                var result = await _context.Users.AddAsync(user);
                //if (!result.Succeeded)
                //    throw new Exception(
                //        string.Join(", ", result.Errors.Select(e => e.Description))
                //    );

                await emailService.SendEmailAsync(
                    dto.Email,
                    "EffortlessQA Invitation",
                    $"You have been invited to join EffortlessQA. Your temporary password is: {tempPassword}"
                );
            }

            // Assign role in project
            var project = await _context.Projects.FirstOrDefaultAsync(p =>
                p.Id == dto.ProjectId && p.TenantId == tenantId && !p.IsDeleted
            );
            if (project == null)
                throw new Exception("Project not found.");

            var role = new Role
            {
                UserId = user.Id,

                RoleType = dto.RoleType,
                TenantId = tenantId
            };
            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                TenantId = user.TenantId
            };
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
                query = query.Where(u => u.Name.Contains(filter) || u.Email.Contains(filter));
            }

            if (!string.IsNullOrEmpty(sort))
            {
                query = sort.ToLower() switch
                {
                    "name" => query.OrderBy(u => u.Name),
                    "-name" => query.OrderByDescending(u => u.Name),
                    "email" => query.OrderBy(u => u.Email),
                    "-email" => query.OrderByDescending(u => u.Email),
                    _ => query.OrderBy(u => u.Name)
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
                    Name = u.Name,
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
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("TenantId", user.TenantId),
                new Claim(
                    ClaimTypes.Role,
                    user.Roles.FirstOrDefault()?.RoleType.ToString() ?? RoleType.Tester.ToString()
                )
            };

            //var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            //var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: null // creds
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
