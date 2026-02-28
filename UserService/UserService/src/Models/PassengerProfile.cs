namespace UserService.Models;

public class PassengerProfile
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string AvatarUrl { get; set; }

    public bool IsBanned { get; set; }
    public int TotalRides { get; set; }
    public int TotalReviews { get; set; }
    public float Rating { get; set; }

    private PassengerProfile()
    {
        AvatarUrl = "";
    }

    public PassengerProfile(int id, string name) : this()
    {
        Id = id;
        Name = name;
    }
}