using AuthService.Enums;

namespace AuthService.Models;

public class Credentials
{
    public int Id { get; private set; }
    public string Email { get; private set; }
    public bool EmailVerified { get; private set; }
    public string PasswordHash { get; private set; }
    public string GoogleOAuthId { get; private set; }
    public string AppleOAuthId { get; private set; }
    public Role Role { get; private set; }
}