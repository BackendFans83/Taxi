using System.Security.Cryptography;

namespace AuthService.Utils;

public class RefreshTokenGenerator : IRefreshTokenGenerator
{
    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        
        var base64 = Convert.ToBase64String(randomBytes);
        return base64.Replace('+', '-').Replace('/', '_').TrimEnd('=');
    }
}