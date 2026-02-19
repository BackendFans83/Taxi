namespace AuthService.DTOs;

public class UnverifiedUser(RegisterRequest registerRequest, string code)
{
    public RegisterRequest RegisterRequest { get; set; } = registerRequest;
    private string Code { get; set; } = code;
}