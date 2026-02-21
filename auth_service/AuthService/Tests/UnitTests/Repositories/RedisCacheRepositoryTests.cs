using AuthService.DTOs;
using AuthService.Repositories;
using Moq;
using StackExchange.Redis;
using AuthService.Enums;
using Role = AuthService.Enums.Role;

namespace Tests.UnitTests.Repositories;

public class RedisCacheRepositoryTests
{
    private readonly Mock<IDatabase> _mockDatabase;
    private readonly RedisCacheRepository _cacheRepository;

    public RedisCacheRepositoryTests()
    {
        var mockConnectionMultiplexer = new Mock<IConnectionMultiplexer>();
        _mockDatabase = new Mock<IDatabase>();
        mockConnectionMultiplexer.Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
            .Returns(_mockDatabase.Object);
        _cacheRepository = new RedisCacheRepository(mockConnectionMultiplexer.Object);
    }

    [Fact]
    public async Task AddRefreshToken_ValidInput_AddsSuccessfully()
    {
        var userId = 1;
        var refreshToken = "refresh_token";
        _mockDatabase
            .Setup(db =>
                db.StringSetAsync($"refresh_token:{refreshToken}", userId.ToString(), It.IsAny<Expiration>()))
            .ReturnsAsync(true);

        var result = await _cacheRepository.AddRefreshToken(userId, refreshToken);

        Assert.True(result);
    }

    [Fact]
    public async Task GetUserIdByRefreshToken_ExistingToken_ReturnsUserId()
    {
        var userId = 1;
        var refreshToken = "refresh_token";
        _mockDatabase.Setup(db => db.StringGetAsync($"refresh_token:{refreshToken}"))
            .ReturnsAsync(userId.ToString());

        var result = await _cacheRepository.GetUserIdByRefreshToken(refreshToken);

        Assert.NotNull(result);
        Assert.Equal(userId, result);
    }

    [Fact]
    public async Task GetUserIdByRefreshToken_NonExistingToken_ReturnsNull()
    {
        var refreshToken = "non_existing_token";
        _mockDatabase.Setup(db => db.StringGetAsync($"refresh_token:{refreshToken}"))
            .ReturnsAsync(RedisValue.Null);

        var result = await _cacheRepository.GetUserIdByRefreshToken(refreshToken);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserIdByRefreshToken_InvalidUserId_ReturnsNull()
    {
        var refreshToken = "refresh_token";
        _mockDatabase.Setup(db => db.StringGetAsync($"refresh_token:{refreshToken}"))
            .ReturnsAsync("invalid_user_id");

        var result = await _cacheRepository.GetUserIdByRefreshToken(refreshToken);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteRefreshToken_ExistingToken_DeletesSuccessfully()
    {
        var refreshToken = "refresh_token_to_delete";
        _mockDatabase.Setup(db => db.KeyDeleteAsync($"refresh_token:{refreshToken}"))
            .ReturnsAsync(true);

        var result = await _cacheRepository.DeleteRefreshToken(refreshToken);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteRefreshToken_NonExistingToken_ReturnsFalse()
    {
        var refreshToken = "non_existing_token";
        _mockDatabase.Setup(db => db.KeyDeleteAsync($"refresh_token:{refreshToken}"))
            .ReturnsAsync(false);

        var result = await _cacheRepository.DeleteRefreshToken(refreshToken);

        Assert.False(result);
    }

    [Fact]
    public async Task CreateUnverifiedUserByToken_ValidInput_CreatesSuccessfully()
    {
        var token = "verification_token";
        var unverifiedUser =
            new UnverifiedUser(new RegisterRequest("name", "test@example.com", "password", "Passenger"), "123456");

        _mockDatabase
            .Setup(db => db.StringSetAsync($"unverified_user:{token}", It.IsAny<RedisValue>(), It.IsAny<Expiration>()))
            .ReturnsAsync(true);

        var result = await _cacheRepository.CreateUnverifiedUserByToken(token, unverifiedUser);

        Assert.True(result);
    }

    [Fact]
    public async Task GetUnverifiedUserByToken_ExistingUser_ReturnsUser()
    {
        var token = "verification_token";
        var unverifiedUser = new UnverifiedUser(new RegisterRequest("name", "test@example.com", "password", "Passenger"),
            "123456");

        var json = System.Text.Json.JsonSerializer.Serialize(unverifiedUser);
        _mockDatabase.Setup(db => db.StringGetAsync($"unverified_user:{token}"))
            .ReturnsAsync(json);

        var result = await _cacheRepository.GetUnverifiedUserByToken(token);

        Assert.NotNull(result);
        Assert.Equal(unverifiedUser.RegisterRequest.Email, result.RegisterRequest.Email);
        Assert.Equal(unverifiedUser.Code, result.Code);
    }

    [Fact]
    public async Task GetUnverifiedUserByToken_NonExistingUser_ReturnsNull()
    {
        var token = "non_existing_token";
        _mockDatabase.Setup(db => db.StringGetAsync($"unverified_user:{token}"))
            .ReturnsAsync(RedisValue.Null);

        var result = await _cacheRepository.GetUnverifiedUserByToken(token);

        Assert.Null(result);
    }
}