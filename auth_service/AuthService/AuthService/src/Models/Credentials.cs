using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AuthService.Enums;

namespace AuthService.Models;

public class Credentials
{
    public int Id { get; private set; }
    public string? Email { get; private set; }
    public bool EmailVerified { get; private set; }
    public string PasswordHash { get; private set; }
    public string? GoogleOAuthId { get; private set; }
    public string? AppleOAuthId { get; private set; }
    public Role Role { get; private set; }

    public Credentials(string email, string passwordHash, Role role, string? googleOAuthId = null,
        string? appleOAuthId = null)
    {
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        GoogleOAuthId = googleOAuthId;
        AppleOAuthId = appleOAuthId;
    }
    
    public Credentials(int id, string email, string passwordHash, Role role, bool emailVerified, string? googleOAuthId = null,
        string? appleOAuthId = null)
    {
        Id = id;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        EmailVerified = emailVerified;
        GoogleOAuthId = googleOAuthId;
        AppleOAuthId = appleOAuthId;
    }
}