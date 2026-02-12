namespace AuthService.DTOs;

public class AuthResponse(string accessToken)
{
    public string AccessToken { get; set; } = accessToken;
}