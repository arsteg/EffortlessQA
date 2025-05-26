using EffortlessQA.Data.Dtos;

namespace EffortlessQA.Api.Services.Interface
{
    public interface IAuthService
    {
        Task<UserDto> RegisterAsync(RegisterDto dto);
        Task<string> LoginAsync(LoginDto dto);
    }
}
