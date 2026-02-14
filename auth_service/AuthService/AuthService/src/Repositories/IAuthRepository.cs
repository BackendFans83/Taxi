using AuthService.Models;

namespace AuthService.Repositories;

public interface IAuthRepository
{
    Task<Credentials?> GetUserCredentialsById(int id);
    Task<Credentials?> GetUserCredentialsByEmail(string email);
    Task<bool> CreateUserCredentials(Credentials credentials);
    Task<bool> ChangePassword(int id, string passwordHash);
    Task<bool> ConfirmEmail(int id);
    Task<bool> DeleteUnverifiedUser(int id);
}