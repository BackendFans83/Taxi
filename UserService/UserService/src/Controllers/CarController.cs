using Microsoft.AspNetCore.Mvc;
using UserService.Services;

namespace UserService.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CarController(ICarService carService) : ControllerBase
{

}
