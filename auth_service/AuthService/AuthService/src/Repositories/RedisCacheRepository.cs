using AuthService.DTOs;
using AuthService.Models;
using StackExchange.Redis;
using System.Text.Json;

namespace AuthService.Repositories;

public class RedisCacheRepository(IConnectionMultiplexer connectionMultiplexer) : ICacheRepository
{
    private readonly IDatabase _database = connectionMultiplexer.GetDatabase();
    private static readonly TimeSpan RefreshTokenExpiration = TimeSpan.FromDays(7);
    private static readonly TimeSpan UnverifiedUserExpiration = TimeSpan.FromMinutes(30);

    public async Task<bool> AddRefreshToken(int userId, string refreshToken)
    {
        var key = $"refresh_token:{refreshToken}";
        return await _database.StringSetAsync(key, userId.ToString(), RefreshTokenExpiration);
    }

    public async Task<int?> GetUserIdByRefreshToken(string refreshToken)
    {
        var key = $"refresh_token:{refreshToken}";
        var value = await _database.StringGetAsync(key);
        return value.HasValue && int.TryParse(value.ToString(), out var userId) ? userId : null;
    }

    public async Task<bool> DeleteRefreshToken(string refreshToken)
    {
        var key = $"refresh_token:{refreshToken}";
        return await _database.KeyDeleteAsync(key);
    }

    public async Task<bool> CreateUnverifiedUserByToken(string token, UnverifiedUser unverifiedUser)
    {
        var key = $"unverified_user:{token}";
        var json = JsonSerializer.Serialize(unverifiedUser);
        return await _database.StringSetAsync(key, json, UnverifiedUserExpiration);
    }

    public async Task<UnverifiedUser?> GetUnverifiedUserByToken(string token)
    {
        var key = $"unverified_user:{token}";
        var value = await _database.StringGetAsync(key);
        return value.HasValue ? JsonSerializer.Deserialize<UnverifiedUser>(value.ToString()) : null;
    }
}