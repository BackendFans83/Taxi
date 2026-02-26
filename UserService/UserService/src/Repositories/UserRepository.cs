using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.Repositories;

public class UserRepository(DbSet<PassengerProfile> passengerProfiles, DbSet<DriverProfile> driverProfiles) : IUserRepository
{

}
