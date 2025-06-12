using EffortlessQA.Data.Dtos;

namespace EffortlessQA.Api.Services.Interface
{
    public interface IAuthService
    {
        Task<UserDto> RegisterAsync(RegisterDto dto);
        Task<string> LoginAsync(LoginDto dto);
        Task<string> OAuthLoginAsync(OAuthLoginDto dto, string provider);
        Task<UserDto> GetUserProfileAsync(Guid userId);
        Task<UserDto> UpdateUserProfileAsync(Guid userId, UpdateUserDto dto);
        Task RequestPasswordResetAsync(PasswordResetRequestDto dto, IEmailService emailService);
        Task ConfirmPasswordResetAsync(PasswordResetConfirmDto dto);
        Task<UserDto> InviteUserAsync(
            InviteUserDto dto,
            string tenantId,
            IEmailService emailService
        );
        Task<PagedResult<UserDto>> GetUsersAsync(
            string tenantId,
            int page,
            int limit,
            string? sort,
            string? filter
        );
        Task DeleteUserAsync(Guid userId, string tenantId);
        Task<TenantDto> GetCurrentTenantAsync();
        Task ChangePasswordAsync(Guid userId, ChangePasswordDto dto);
    }
}
