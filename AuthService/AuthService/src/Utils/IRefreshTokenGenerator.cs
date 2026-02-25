namespace AuthService.Utils;

public interface IRefreshTokenGenerator
{
    string GenerateRefreshToken();
}