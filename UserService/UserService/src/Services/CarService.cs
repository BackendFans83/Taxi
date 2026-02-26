using UserService.Repositories;

namespace UserService.Services;

public class CarService(ICarRepository carRepository) : ICarService
{
    
}
