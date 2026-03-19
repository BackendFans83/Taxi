namespace RideService.DTOs;

public class CreateRideDto
{
    public int PassengerId { get; set; }
    
    public double PickupLatitude { get; set; }
    public double PickupLongitude { get; set; }
    public double DropOffLatitude { get; set; }
    public double DropOffLongitude { get; set; }
    
    public string PickupAddress { get; set; }
    public string DropOffAddress { get; set; }
}