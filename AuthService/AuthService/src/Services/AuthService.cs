using AuthService.DTOs;
using AuthService.Enums;
using AuthService.Models;
using AuthService.Producers;
using AuthService.Repositories;
using AuthService.Utils;

namespace AuthService.Services;

public class AuthService(
    IAuthRepository authRepository,
    ICacheRepository cacheRepository,
    IAccessTokenGenerator accessTokenGenerator,
    IRefreshTokenGenerator refreshTokenGenerator,
    IKafkaProducer kafkaProducer)
    : IAuthService
{
    public async Task<Result<AuthResponse>> Register(RegisterRequest registerRequest)
    {
        registerRequest.Email = registerRequest.Email.ToLower();
        var existingUser = await authRepository.GetUserCredentialsByEmail(registerRequest.Email);

        if (existingUser != null)
            return Result<AuthResponse>.Failure(409, "User with this email already exists");
        if (!Enum.TryParse<Role>(registerRequest.Role, true, out var role))
            return Result<AuthResponse>.Failure(400, "Invalid role specified");

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password, workFactor: 9);
        var credentials = new Credentials(registerRequest.Email, hashedPassword, role);

        var createUserResult = await authRepository.CreateUserCredentials(credentials);
        if (!createUserResult)
            return Result<AuthResponse>.Failure(500, "Failed to create user account");

        var savedUser = await authRepository.GetUserCredentialsByEmail(registerRequest.Email);
        if (savedUser == null)
            return Result<AuthResponse>.Failure(500, "Failed to retrieve created user");

        var kafkaResult = await kafkaProducer.SendUserRegisteredEventAsync(new CreateUserDto(savedUser.Id,
            registerRequest.Name, registerRequest.Role));
        
        var accessToken = accessTokenGenerator.GenerateAccessToken(savedUser.Id, savedUser.Role);
        var authResponse = new AuthResponse(savedUser.Id, accessToken);

        return Result<AuthResponse>.Success(authResponse);
    }

    public async Task<Result<AuthResponse>> Login(LoginRequest loginRequest)
    {
        loginRequest.Email = loginRequest.Email.ToLower();
        var credentials = await authRepository.GetUserCredentialsByEmail(loginRequest.Email);

        if (credentials == null)
            return Result<AuthResponse>.Failure(401, "Invalid email or password");

        if (!BCrypt.Net.BCrypt.Verify(loginRequest.Password, credentials.PasswordHash))
            return Result<AuthResponse>.Failure(401, "Invalid email or password");

        var accessToken = accessTokenGenerator.GenerateAccessToken(credentials.Id, credentials.Role);
        var authResponse = new AuthResponse(credentials.Id, accessToken);

        return Result<AuthResponse>.Success(authResponse);
    }

    public async Task<Result> Logout(string refreshToken)
    {
        var userId = await cacheRepository.GetUserIdByRefreshToken(refreshToken);

        if (userId == null)
            return Result.Failure(401, "Invalid refresh token");

        var deleted = await cacheRepository.DeleteRefreshToken(refreshToken);

        return deleted ? Result.Success() : Result.Failure(500, "Failed to logout");
    }

    public Task<Result> Refresh(string refreshToken)
    {
        throw new NotImplementedException();
    }

    public Task<Result<string>> SendVerificationCode(string email)
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

    public async Task<string?> GenerateRefreshToken(int id)
    {
        var refreshToken = refreshTokenGenerator.GenerateRefreshToken();
        var cacheResult = await cacheRepository.AddRefreshToken(id, refreshToken);
        return !cacheResult ? null : refreshToken;
    }
}