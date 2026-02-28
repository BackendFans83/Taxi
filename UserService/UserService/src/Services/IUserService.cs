using UserService.DTOs;

namespace UserService.Services;

public interface IUserService
{
    Task<bool> CreateUser(CreateUserDto user);
}
