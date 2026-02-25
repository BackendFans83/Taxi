namespace AuthService.DTOs;

public class AuthResponse(int id, string accessToken)
{
    public int Id { get; } = id;
    public string AccessToken { get; } = accessToken;
}