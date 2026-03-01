using UserService.Models;

namespace UserService.DTOs;

public class PassengerProfileDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string AvatarUrl { get; set; }
    public bool IsBanned { get; set; }
    public int TotalRides { get; set; }
    public int TotalReviews { get; set; }
    public float Rating { get; set; }

    public PassengerProfileDto() { }

    public PassengerProfileDto(PassengerProfile profile)
    {
        Id = profile.Id;
        Name = profile.Name;
        AvatarUrl = profile.AvatarUrl;
        IsBanned = profile.IsBanned;
        TotalRides = profile.TotalRides;
        TotalReviews = profile.TotalReviews;
        Rating = profile.Rating;
    }
}
