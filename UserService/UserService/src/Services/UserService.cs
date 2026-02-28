using UserService.DTOs;
using UserService.Repositories;

namespace UserService.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<bool> CreateUser(CreateUserDto user)
    {
        throw new NotImplementedException();
    }
}
