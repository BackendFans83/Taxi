using UserService.Enums;

namespace UserService.Models;

public class Car
{
    public int Id { get; set; }
    public int DriverId { get; set; }

    public string Brand { get; set; }
    public string Model { get; set; }
    public string Color { get; set; }
    public string Number { get; set; }
    public int Year { get; set; }
    public CarClass Class { get; set; }
}
