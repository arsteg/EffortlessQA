using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EffortlessQA.Api.Services.Interface;
using EffortlessQA.Data;
using EffortlessQA.Data.Dtos;
using EffortlessQA.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace EffortlessQA.Api.Services.Implementation
{
    public class AuthService : IAuthService
    {
        //private readonly UserManager<User> _userManager;
        //private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly EffortlessQAContext _context;

        public AuthService(
            //UserManager<User> userManager,
            //SignInManager<User> signInManager,
            EffortlessQAContext context,
            IConfiguration configuration
        )
        {
            //_userManager = userManager;
            //_signInManager = signInManager;
            _context = context;
            _configuration = configuration;
        }

        public async Task<UserDto> RegisterAsync(RegisterDto dto)
        {
            var user = new User
            {
                Email = dto.Email,
                Name = dto.Name,
                TenantId = dto.TenantId
            };

            var result = await _context.Users.AddAsync(user);
            //if (!result.Succeeded)
            //    throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            //await _userManager.AddToRoleAsync(user, RoleType.Tester.ToString());

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                TenantId = user.TenantId
            };
        }

        public async Task<string> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users.FindAsync(dto.Email);
            if (user == null)
                throw new Exception("Invalid email or password.");

            //var result = await _signInManager.PasswordSignInAsync(user, dto.Password, false, false);
            //if (!result.Succeeded)
            //    throw new Exception("Invalid email or password.");

            var token = GenerateJwtToken(user);
            return token;
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

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
