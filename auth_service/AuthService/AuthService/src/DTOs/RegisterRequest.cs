namespace AuthService.DTOs;

public class RegisterRequest(string name, string email, string password, string role)
{
    public string Name { get; set; } = name;
    public string Email { get; set; } = email;
    public string Password { get; set; } = password;
    public string Role { get; set; } = role;
}