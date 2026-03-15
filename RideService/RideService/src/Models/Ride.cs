
namespace RideService.Models;

public class Ride
{
    public int Id { get; set; }
    public int PassengerId { get; set; }
    public int DriverId { get; set; }
    public RideStatus Status { get; set; }
    
    public double PickupLatitude { get; set; }
    public double PickupLongitude { get; set; }
    public double DropOffLatitude { get; set; }
    public double DropOffLongitude { get; set; }
    
    public string PickupAddress { get; set; }
    public string DropOffAddress { get; set; }
    public decimal Price { get; set; }
    public double Distance { get; set; }
    
    public DateTime RequestedAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}