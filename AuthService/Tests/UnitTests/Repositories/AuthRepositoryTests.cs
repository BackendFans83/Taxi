using AuthService.Data;
using AuthService.Enums;
using AuthService.Models;
using AuthService.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.UnitTests.Repositories;

public class AuthRepositoryTests
{
    private readonly ApplicationDbContext _context;
    private readonly AuthRepository _authRepository;

    public AuthRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;

        _context = new ApplicationDbContext(options);
        _authRepository = new AuthRepository(_context);
        
        SeedData();
    }

    private void SeedData()
    {
        _context.Credentials.AddRange(
            new Credentials(1, "user1@example.com", "hashed_password_1", Role.Passenger),
            new Credentials(2, "user2@example.com", "hashed_password_2", Role.Rider),
            new Credentials(3, "admin@example.com", "hashed_password_3", Role.Admin)
        );
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetUserCredentialsById_ExistingUser_ReturnsUser()
    {
        var result = await _authRepository.GetUserCredentialsById(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("user1@example.com", result.Email);
    }

    [Fact]
    public async Task GetUserCredentialsById_NonExistingUser_ReturnsNull()
    {
        var result = await _authRepository.GetUserCredentialsById(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserCredentialsByEmail_ExistingUser_ReturnsUser()
    {
        var result = await _authRepository.GetUserCredentialsByEmail("user2@example.com");

        Assert.NotNull(result);
        Assert.Equal(2, result.Id);
        Assert.Equal("user2@example.com", result.Email);
    }

    [Fact]
    public async Task GetUserCredentialsByEmail_NonExistingUser_ReturnsNull()
    {
        var result = await _authRepository.GetUserCredentialsByEmail("nonexistent@example.com");

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateUserCredentials_ValidUser_CreatesSuccessfully()
    {
        const string newUserEmail = "newuser@example.com";
        var newUser = new Credentials(newUserEmail, "new_hashed_password", Role.Passenger);
        
        var result = await _authRepository.CreateUserCredentials(newUser);
        
        Assert.True(result);
        var savedUser = await _context.Credentials.FirstOrDefaultAsync(u => u.Email == newUserEmail);
        Assert.NotNull(savedUser);
        Assert.Equal(newUserEmail, savedUser.Email);
    }

    [Fact]
    public async Task ChangePassword_ValidRequest_ChangesSuccessfully()
    {
        const int userId = 1;
        const string newPassword = "updated_password";
        
        var result = await _authRepository.ChangePassword(userId, newPassword);
        
        Assert.True(result);
        var updatedUser = await _context.Credentials.FindAsync(userId);
        Assert.NotNull(updatedUser);
        Assert.Equal(newPassword, updatedUser.PasswordHash);
    }

    [Fact]
    public async Task ChangePassword_UserNotFound_ReturnsFalse()
    {
        const int userId = 999;
        const string newPassword = "updated_password";
        
        var result = await _authRepository.ChangePassword(userId, newPassword);

        Assert.False(result);
    }
}