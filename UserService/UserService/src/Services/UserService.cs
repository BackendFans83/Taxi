using UserService.Repositories;

namespace UserService.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
    
}
