using AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Repositories;

public class AuthRepository(DbContext authDbContext) : IAuthRepository
{
    public async Task<Credentials?> GetUserCredentialsById(int id)
    {
        return await authDbContext.Set<Credentials>().FindAsync(id);
    }

    public async Task<Credentials?> GetUserCredentialsByEmail(string email)
    {
        return await authDbContext.Set<Credentials>().FirstOrDefaultAsync(c => c.Email == email);
    }

    public async Task<bool> CreateUserCredentials(Credentials credentials)
    {
        authDbContext.Set<Credentials>().Add(credentials);
        var result = await authDbContext.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> ChangePassword(int id, string passwordHash)
    {
        var user = await authDbContext.Set<Credentials>().FindAsync(id);
        if (user == null) return false;
        
        user.ChangePassword(passwordHash);
        var result = await authDbContext.SaveChangesAsync();
        return result > 0;
    }
}
