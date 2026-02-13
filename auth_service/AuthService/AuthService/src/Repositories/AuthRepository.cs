using AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Repositories;

public class AuthRepository(DbContext authDbContext) : IAuthRepository
{
    private readonly DbContext _authDbContext = authDbContext;

    public Task<Credentials?> GetUserCredentialsById(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Credentials?> GetUserCredentialsByEmail(string email)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CreateUserCredentials(Credentials credentials)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ChangePassword(int id, string password)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ConfirmEmail(int id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteUnverifiedUser(int id)
    {
        throw new NotImplementedException();
    }
}