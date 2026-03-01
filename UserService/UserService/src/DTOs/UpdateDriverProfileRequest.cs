namespace UserService.DTOs;

public class UpdateDriverProfileRequest
{
    public string Name { get; set; }
    public string AvatarUrl { get; set; }
    public string? LicenseNumber { get; set; }
    public DateOnly LicenseExpiryDate { get; set; }
    public int CurrentCarId { get; set; }
}
