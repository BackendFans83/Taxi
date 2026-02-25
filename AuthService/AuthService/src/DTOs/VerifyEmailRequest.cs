namespace AuthService.DTOs;

public class VerifyEmailRequest
{
    public string Token { get; set; }
    public string Email { get; set; }
    public string Code { get; set; }
}