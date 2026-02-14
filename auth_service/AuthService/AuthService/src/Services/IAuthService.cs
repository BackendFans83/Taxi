using AuthService.DTOs;

namespace AuthService.Services;

public interface IAuthService
{
    Task<Result<AuthResponse>> Register(RegisterRequest registerRequest);
    Task<Result<AuthResponse>> Login(LoginRequest loginRequest);
    Task<Result> Logout(string refreshToken);
    Task<Result> Refresh(string refreshToken);
    Task<Result> SendVerificationCode(string email);
    Task<Result> VerifyEmail(string email, string code);
    Task<Result> ChangePassword(int userId, string oldPassword, string newPassword);
    Task<string?> GenerateRefreshToken(int id);
}