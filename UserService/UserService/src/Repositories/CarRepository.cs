using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.Repositories;

public class CarRepository(DbSet<Car> cars) : ICarRepository
{

}
