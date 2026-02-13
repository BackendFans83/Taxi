using AuthService.Enums;

namespace AuthService.Utils;

public class JwtAccessTokenGenerator(IConfiguration configuration) : IAccessTokenGenerator
{
    private readonly string _secretKey = configuration.GetValue<string>("Jwt:SecretKey")!;

    public string GenerateAccessToken(int userId, Role role)
    {
        throw new NotImplementedException();
    }
}