using UserService.Data;

namespace UserService.Repositories;

public class UserRepository(ApplicationDbContext dbContext) : IUserRepository
{

}
