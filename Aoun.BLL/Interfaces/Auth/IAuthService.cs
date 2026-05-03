using Aoun.BLL.DTOs.Auth;
using System.Threading.Tasks;

namespace Aoun.BLL.Interfaces.Auth;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto model);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto model);
    Task<AuthResponseDto> ForgotPasswordAsync(ForgotPasswordDto model);
    Task<AuthResponseDto> VerifyEmailAsync(VerifyEmailDto model);
    Task<AuthResponseDto> SocialLoginAsync(SocialLoginDto model);
}
