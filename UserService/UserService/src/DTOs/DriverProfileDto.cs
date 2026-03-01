using UserService.Models;

namespace UserService.DTOs;

public class DriverProfileDto
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

    public DriverProfileDto() { }

    public DriverProfileDto(DriverProfile profile)
    {
        Id = profile.Id;
        Name = profile.Name;
        AvatarUrl = profile.AvatarUrl;
        IsBanned = profile.IsBanned;
        TotalRides = profile.TotalRides;
        TotalReviews = profile.TotalReviews;
        Rating = profile.Rating;
        LicenseNumber = profile.LicenseNumber;
        LicenseExpiryDate = profile.LicenseExpiryDate;
        CurrentCarId = profile.CurrentCarId;
    }
}
