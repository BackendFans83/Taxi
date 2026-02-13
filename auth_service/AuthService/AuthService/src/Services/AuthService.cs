using AuthService.DTOs;

namespace AuthService.Services;

public class AuthService : IAuthService
{
    public Task<Result<AuthResponse>> Register(RegisterRequest registerRequest)
    {
        throw new NotImplementedException();
    }

    public Task<Result<AuthResponse>> Login(LoginRequest loginRequest)
    {
        throw new NotImplementedException();
    }

    public Task<Result> Logout(string refreshToken)
    {
        throw new NotImplementedException();
    }

    public Task<Result> Refresh(string refreshToken)
    {
        throw new NotImplementedException();
    }

    public Task<Result> SendVerificationCode(string email)
    {
        throw new NotImplementedException();
    }

    public Task<Result> VerifyEmail(string email, string code)
    {
        throw new NotImplementedException();
    }

    public Task<Result> ChangePassword(int userId, string oldPassword, string newPassword)
    {
        throw new NotImplementedException();
    }
}