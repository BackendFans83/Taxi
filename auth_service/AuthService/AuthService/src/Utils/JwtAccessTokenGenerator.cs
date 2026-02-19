using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthService.Enums;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Utils;

public class JwtAccessTokenGenerator(IConfiguration configuration) : IAccessTokenGenerator
{
    private readonly string _issuer = configuration.GetValue<string>("Jwt:Issuer")!;
    private readonly string _audience = configuration.GetValue<string>("Jwt:Audience")!;
    private readonly string _secretKey = configuration.GetValue<string>("Jwt:SecretKey")!;
    private readonly int _expirationMinutes = Math.Max(configuration.GetValue<int>("Jwt:ExpirationMinutes"), 1);

    public string GenerateAccessToken(int userId, Role role)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new (JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(ClaimTypes.Role, role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}