using RideService.Models;

namespace RideService.DTOs;

public class RideDto
{
    public int Id { get; set; }
    public int PassengerId { get; set; }
    
    public double PickupLatitude { get; set; }
    public double PickupLongitude { get; set; }
    public double DropOffLatitude { get; set; }
    public double DropOffLongitude { get; set; }
    public RideStatus Status { get; set; }

    public RideDto(Ride ride)
    {
        Id = ride.Id;
        PassengerId = ride.PassengerId;
        PickupLatitude = ride.PickupLatitude;
        PickupLongitude = ride.PickupLongitude;
        DropOffLatitude = ride.DropOffLatitude;
        DropOffLongitude = ride.DropOffLongitude;
        Status = ride.Status;
    }
}