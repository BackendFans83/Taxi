namespace UserService.Models;

public class DriverProfile
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string AvatarUrl { get; set; }

    public bool IsBanned { get; set; }
    public int TotalRides { get; set; }
    public int TotalReviews { get; set; }
    public float Rating { get; set; }

    public string? LicenseNumber { get; set; }
    public DateOnly LicenseExpiryDate { get; set; }

    public int CurrentCarId { get; set; }

    private DriverProfile()
    {
        AvatarUrl = "";
    }

    public DriverProfile(int id, string name) : this()
    {
        Id = id;
        Name = name;
    }
}