using AuthService.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[ApiController, Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
    {
        throw new NotImplementedException();
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody]LoginRequest loginRequest)
    {
        throw new NotImplementedException();
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
}