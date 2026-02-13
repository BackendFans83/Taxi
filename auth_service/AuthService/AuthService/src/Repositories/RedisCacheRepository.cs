namespace AuthService.Repositories;

public class RedisCacheRepository : ICacheRepository
{
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