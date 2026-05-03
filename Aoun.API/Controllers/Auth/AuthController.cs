using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Aoun.BLL.DTOs.Auth;
using Aoun.BLL.Interfaces.Auth;

namespace Aoun.API.Controllers.Auth;

[ApiController]
[Route("api/[controller]")]

public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        => Ok(await _authService.RegisterAsync(request));

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        => Ok(await _authService.LoginAsync(request));

    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto request)
        => Ok(await _authService.ForgotPasswordAsync(request));

    [AllowAnonymous]
    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto request)
        => Ok(await _authService.VerifyEmailAsync(request));

    [AllowAnonymous]
    [HttpPost("social-login")]
    public async Task<IActionResult> SocialLogin([FromBody] SocialLoginDto request)
        => Ok(await _authService.SocialLoginAsync(request));
}




