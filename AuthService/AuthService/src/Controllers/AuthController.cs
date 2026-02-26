using AuthService.DTOs;
using AuthService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[ApiController, Route("api/v1/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
    {
        var result = await authService.Register(registerRequest);

        if (!result.IsSuccess)
            return GetErrorResult(result);

        var actionResult = await CreateRefreshTokenInCookie(result);
        return actionResult ?? Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        var result = await authService.Login(loginRequest);

        if (!result.IsSuccess)
            return GetErrorResult(result);

        var actionResult = await CreateRefreshTokenInCookie(result);
        return actionResult ?? Ok(result);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        throw new NotImplementedException();
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        throw new NotImplementedException();
    }

    [HttpPost("email/send-verification-code")]
    public async Task<IActionResult> SendVerificationCode([FromBody] EmailDto emailDto)
    {
        throw new NotImplementedException();
    }

    [HttpPost("email/verify")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest verifyEmailRequest)
    {
        throw new NotImplementedException();
    }

    [HttpPatch("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest changePasswordRequest)
    {
        throw new NotImplementedException();
    }

    private async Task<IActionResult?> CreateRefreshTokenInCookie(Result<AuthResponse> result)
    {
        var refreshToken = await authService.GenerateRefreshToken(result.Value!.Id);
        if (refreshToken == null)
            return StatusCode(500, "Refresh token not created");
        HttpContext.Response.Cookies.Append("RefreshToken", refreshToken);
        return null;
    }

    private IActionResult GetErrorResult(Result<AuthResponse> result)
    {
        var message = result.ErrorMessage;
        return result.StatusCode switch
        {
            400 => BadRequest(message),
            409 => Conflict(message),
            500 => StatusCode(500, message),
            _ => StatusCode(result.StatusCode, message)
        };
    }
}