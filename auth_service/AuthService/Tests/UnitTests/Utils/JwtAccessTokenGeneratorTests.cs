using AuthService.Enums;
using AuthService.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Tests.UnitTests.Utils;

public class JwtAccessTokenGeneratorTests
{
    private readonly JwtAccessTokenGenerator _jwtAccessTokenGenerator;
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expirationMinutes;

    public JwtAccessTokenGeneratorTests()
    {
        _secretKey = "super_secret_key_for_testing_purposes_only_super_long";
        _issuer = "test-issuer";
        _audience = "test-audience";
        _expirationMinutes = 60;

        var inMemorySettings = new Dictionary<string, string?>
        {
            { "Jwt:SecretKey", _secretKey },
            { "Jwt:Issuer", _issuer },
            { "Jwt:Audience", _audience },
            { "Jwt:ExpirationMinutes", _expirationMinutes.ToString() },
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _jwtAccessTokenGenerator = new JwtAccessTokenGenerator(configuration);
    }

    [Fact]
    public void GenerateAccessToken_ValidInput_ReturnsValidToken()
    {
        var userId = 1;
        var role = Role.Passenger;

        var token = _jwtAccessTokenGenerator.GenerateAccessToken(userId, role);

        Assert.NotNull(token);
        Assert.NotEmpty(token);

        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey)),
            ValidateIssuer = true,
            ValidIssuer = _issuer,
            ValidateAudience = true,
            ValidAudience = _audience,
            ClockSkew = TimeSpan.Zero
        };

        var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

        Assert.IsType<JwtSecurityToken>(validatedToken);
        Assert.Equal(userId.ToString(), principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        Assert.Equal(role.ToString(), principal.FindFirst(ClaimTypes.Role)?.Value);
    }

    [Fact]
    public void GenerateAccessToken_DifferentUsers_ReturnsDifferentTokens()
    {
        var userId1 = 1;
        var userId2 = 2;
        var role = Role.Passenger;

        var token1 = _jwtAccessTokenGenerator.GenerateAccessToken(userId1, role);
        var token2 = _jwtAccessTokenGenerator.GenerateAccessToken(userId2, role);

        Assert.NotEqual(token1, token2);
    }

    [Fact]
    public void GenerateAccessToken_DifferentRoles_ReturnsDifferentTokens()
    {
        var userId = 1;
        var role1 = Role.Passenger;
        var role2 = Role.Admin;

        var token1 = _jwtAccessTokenGenerator.GenerateAccessToken(userId, role1);
        var token2 = _jwtAccessTokenGenerator.GenerateAccessToken(userId, role2);

        Assert.NotEqual(token1, token2);
    }

    [Fact]
    public void GenerateAccessToken_WithRiderRole_ReturnsValidToken()
    {
        var userId = 5;
        var role = Role.Rider;

        var token = _jwtAccessTokenGenerator.GenerateAccessToken(userId, role);

        Assert.NotNull(token);
        Assert.NotEmpty(token);

        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey)),
            ValidateIssuer = true,
            ValidIssuer = _issuer,
            ValidateAudience = true,
            ValidAudience = _audience,
            ClockSkew = TimeSpan.Zero
        };

        var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

        Assert.IsType<JwtSecurityToken>(validatedToken);
        Assert.Equal(userId.ToString(), principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        Assert.Equal(role.ToString(), principal.FindFirst(ClaimTypes.Role)?.Value);
    }
}