namespace AuthService.Repositories;

public interface ICacheRepository
{
    Task<bool> AddRefreshToken(int userId, string refreshToken);
    Task<int?> GetUserIdByRefreshToken(string refreshToken);
    Task<bool> DeleteRefreshToken(string refreshToken);
}