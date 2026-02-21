using AuthService.Utils;

namespace Tests.UnitTests.Utils;

public class RefreshTokenGeneratorTests
{
    private readonly RefreshTokenGenerator _refreshTokenGenerator = new();

    [Fact]
    public void GenerateRefreshToken_ReturnsNonEmptyString()
    {
        var token = _refreshTokenGenerator.GenerateRefreshToken();

        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }

    [Fact]
    public void GenerateRefreshToken_MultipleCalls_ReturnsDifferentTokens()
    {
        var tokens = Enumerable.Range(0, 100)
            .Select(_ => _refreshTokenGenerator.GenerateRefreshToken())
            .ToList();

        var distinctTokens = tokens.Distinct().ToList();
        Assert.Equal(100, distinctTokens.Count);
    }

    [Fact]
    public void GenerateRefreshToken_TokensHaveExpectedLength()
    {
        var token = _refreshTokenGenerator.GenerateRefreshToken();

        Assert.True(token.Length >= 32);
    }

    [Fact]
    public void GenerateRefreshToken_TokensAreUrlSafe()
    {
        var token = _refreshTokenGenerator.GenerateRefreshToken();

        var isValid = System.Text.RegularExpressions.Regex.IsMatch(token, @"^[a-zA-Z0-9_-]+$");
        Assert.True(isValid);
    }

    [Fact]
    public void GenerateRefreshToken_DoesNotContainUrlUnsafeCharacters()
    {
        var tokens = Enumerable.Range(0, 50)
            .Select(_ => _refreshTokenGenerator.GenerateRefreshToken())
            .ToList();

        foreach (var token in tokens)
        {
            Assert.DoesNotContain('+', token);
            Assert.DoesNotContain('/', token);
            Assert.DoesNotContain('=', token);
        }
    }
}