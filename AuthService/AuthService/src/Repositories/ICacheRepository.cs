using AuthService.DTOs;
using AuthService.Models;

namespace AuthService.Repositories;

public interface ICacheRepository
{
    Task<bool> AddRefreshToken(int userId, string refreshToken);
    Task<int?> GetUserIdByRefreshToken(string refreshToken);
    Task<bool> DeleteRefreshToken(string refreshToken);
    Task<bool> CreateUnverifiedUserByToken(string token, UnverifiedUser unverifiedUser);
    Task<UnverifiedUser?> GetUnverifiedUserByToken(string token);
}