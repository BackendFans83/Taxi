using StackExchange.Redis;

namespace AuthService.Repositories;

public class RedisCacheRepository(IConnectionMultiplexer connectionMultiplexer) : ICacheRepository
{
    private readonly IConnectionMultiplexer _connectionMultiplexer = connectionMultiplexer;

    public Task<bool> AddRefreshToken(int userId, string refreshToken)
    {
        throw new NotImplementedException();
    }

    public Task<int?> GetUserIdByRefreshToken(string refreshToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteRefreshToken(string refreshToken)
    {
        throw new NotImplementedException();
    }
}