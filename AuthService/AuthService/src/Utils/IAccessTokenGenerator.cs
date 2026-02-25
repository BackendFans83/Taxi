using AuthService.Enums;

namespace AuthService.Utils;

public interface IAccessTokenGenerator
{
    string GenerateAccessToken(int userId, Role role);
}