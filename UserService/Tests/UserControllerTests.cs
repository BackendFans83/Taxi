using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using UserService.Controllers;
using UserService.DTOs;
using UserService.Services;

namespace Tests;

public class UserControllerTests
{
    private readonly Mock<IUserService> mockService;
    private readonly UserController controller;

    public UserControllerTests()
    {
        mockService = new Mock<IUserService>();
        controller = new UserController(mockService.Object);
    }

    private void SetClaims(List<Claim> claims)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    #region GetCurrentUserProfile Tests

    [Fact]
    public async Task GetCurrentUserProfile_PassengerRole_ReturnsOkWithPassengerProfile()
    {
        var passengerDto = new PassengerProfileDto
        {
            Id = 1,
            Name = "Test Passenger",
            AvatarUrl = "avatar.jpg",
            IsBanned = false,
            TotalRides = 10,
            TotalReviews = 5,
            Rating = 4.5f
        };
        mockService.Setup(s => s.GetPassengerProfileAsync(1))
            .ReturnsAsync(Result<PassengerProfileDto>.Success(passengerDto));

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, "1"),
            new(ClaimTypes.Role, "Passenger")
        };
        SetClaims(claims);

        var result = await controller.GetCurrentUserProfile();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedProfile = Assert.IsType<PassengerProfileDto>(okResult.Value);
        Assert.Equal("Test Passenger", returnedProfile.Name);
    }

    [Fact]
    public async Task GetCurrentUserProfile_DriverRole_ReturnsOkWithDriverProfile()
    {
        var driverDto = new DriverProfileDto
        {
            Id = 1,
            Name = "Test Driver",
            AvatarUrl = "avatar.jpg",
            IsBanned = false,
            TotalRides = 20,
            TotalReviews = 8,
            Rating = 4.8f,
            LicenseNumber = "ABC123",
            LicenseExpiryDate = new DateOnly(2027, 1, 1),
            CurrentCarId = 5
        };
        mockService.Setup(s => s.GetDriverProfileAsync(1))
            .ReturnsAsync(Result<DriverProfileDto>.Success(driverDto));

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, "1"),
            new(ClaimTypes.Role, "Driver")
        };
        SetClaims(claims);

        var result = await controller.GetCurrentUserProfile();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedProfile = Assert.IsType<DriverProfileDto>(okResult.Value);
        Assert.Equal("Test Driver", returnedProfile.Name);
        Assert.Equal("ABC123", returnedProfile.LicenseNumber);
    }

    [Fact]
    public async Task GetCurrentUserProfile_ProfileNotFound_ReturnsNotFound()
    {
        mockService.Setup(s => s.GetPassengerProfileAsync(1))
            .ReturnsAsync(Result<PassengerProfileDto>.Failure(404, "Profile not found"));

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, "1"),
            new(ClaimTypes.Role, "Passenger")
        };
        SetClaims(claims);

        var result = await controller.GetCurrentUserProfile();

        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(404, statusCodeResult.StatusCode);
        Assert.Equal("Profile not found", statusCodeResult.Value);
    }

    [Fact]
    public async Task GetCurrentUserProfile_NoUserIdClaim_ReturnsUnauthorized()
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Role, "Passenger")
        };
        SetClaims(claims);

        var result = await controller.GetCurrentUserProfile();

        Assert.IsType<UnauthorizedResult>(result.Result);
    }

    [Fact]
    public async Task GetCurrentUserProfile_NoRoleClaim_ReturnsUnauthorized()
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, "1")
        };
        SetClaims(claims);

        var result = await controller.GetCurrentUserProfile();

        Assert.IsType<UnauthorizedResult>(result.Result);
    }

    [Fact]
    public async Task GetCurrentUserProfile_AdminRole_ReturnsPassengerProfile()
    {
        var passengerDto = new PassengerProfileDto { Id = 1, Name = "Admin User", AvatarUrl = "" };
        mockService.Setup(s => s.GetPassengerProfileAsync(1))
            .ReturnsAsync(Result<PassengerProfileDto>.Success(passengerDto));

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, "1"),
            new(ClaimTypes.Role, "Admin")
        };
        SetClaims(claims);

        var result = await controller.GetCurrentUserProfile();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedProfile = Assert.IsType<PassengerProfileDto>(okResult.Value);
        Assert.Equal(passengerDto.Id, returnedProfile.Id);
        Assert.Equal(passengerDto.Name, returnedProfile.Name);
    }

    #endregion

    #region UpdateCurrentUserProfile Tests

    [Fact]
    public async Task UpdateCurrentUserProfile_PassengerRole_ReturnsOkWithUpdatedProfile()
    {
        var request = new UpdatePassengerProfileRequest { Name = "Updated Name", AvatarUrl = "new.jpg" };
        var updatedDto = new PassengerProfileDto
        {
            Id = 1,
            Name = request.Name,
            AvatarUrl = request.AvatarUrl,
            IsBanned = false,
            TotalRides = 10,
            TotalReviews = 5,
            Rating = 4.5f
        };
        mockService.Setup(s => s.UpdatePassengerProfileAsync(1, It.IsAny<UpdatePassengerProfileRequest>()))
            .ReturnsAsync(Result<PassengerProfileDto>.Success(updatedDto));

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, "1"),
            new(ClaimTypes.Role, "Passenger")
        };
        SetClaims(claims);

        var result = await controller.UpdateCurrentUserProfile(request);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedProfile = Assert.IsType<PassengerProfileDto>(okResult.Value);
        Assert.Equal(request.Name, returnedProfile.Name);
    }

    [Fact]
    public async Task UpdateCurrentUserProfile_DriverRole_ReturnsOkWithUpdatedProfile()
    {
        var request = new UpdateDriverProfileRequest
        {
            Name = "Updated Driver",
            AvatarUrl = "new.jpg",
            LicenseNumber = "XYZ789",
            LicenseExpiryDate = new DateOnly(2028, 1, 1),
            CurrentCarId = 10
        };
        var updatedDto = new DriverProfileDto
        {
            Id = 1,
            Name = request.Name,
            AvatarUrl = request.AvatarUrl,
            IsBanned = false,
            TotalRides = 25,
            TotalReviews = 10,
            Rating = 4.9f,
            LicenseNumber = request.LicenseNumber,
            LicenseExpiryDate = request.LicenseExpiryDate,
            CurrentCarId = request.CurrentCarId
        };
        mockService.Setup(s => s.UpdateDriverProfileAsync(1, It.IsAny<UpdateDriverProfileRequest>()))
            .ReturnsAsync(Result<DriverProfileDto>.Success(updatedDto));

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, "1"),
            new(ClaimTypes.Role, "Driver")
        };
        SetClaims(claims);

        var result = await controller.UpdateCurrentUserProfile(request);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedProfile = Assert.IsType<DriverProfileDto>(okResult.Value);
        Assert.Equal(request.Name, returnedProfile.Name);
        Assert.Equal(request.AvatarUrl, returnedProfile.AvatarUrl);
        Assert.Equal(request.LicenseNumber, returnedProfile.LicenseNumber);
        Assert.Equal(request.LicenseExpiryDate, returnedProfile.LicenseExpiryDate);
        Assert.Equal(request.CurrentCarId, returnedProfile.CurrentCarId);
    }

    [Fact]
    public async Task UpdateCurrentUserProfile_InvalidRequestType_ReturnsBadRequest()
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, "1"),
            new(ClaimTypes.Role, "Passenger")
        };
        SetClaims(claims);
        var invalidRequest = new UpdateDriverProfileRequest();

        var result = await controller.UpdateCurrentUserProfile(invalidRequest);

        Assert.IsType<BadRequestResult>(result.Result);
    }

    [Fact]
    public async Task UpdateCurrentUserProfile_ProfileNotFound_ReturnsNotFound()
    {
        mockService.Setup(s => s.UpdatePassengerProfileAsync(1, It.IsAny<UpdatePassengerProfileRequest>()))
            .ReturnsAsync(Result<PassengerProfileDto>.Failure(404, "Profile not found"));

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, "1"),
            new(ClaimTypes.Role, "Passenger")
        };
        SetClaims(claims);
        var request = new UpdatePassengerProfileRequest { Name = "Updated", AvatarUrl = "new.jpg" };

        var result = await controller.UpdateCurrentUserProfile(request);

        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(404, statusCodeResult.StatusCode);
        Assert.Equal("Profile not found", statusCodeResult.Value);
    }

    [Fact]
    public async Task UpdateCurrentUserProfile_NoUserIdClaim_ReturnsUnauthorized()
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Role, "Passenger")
        };
        SetClaims(claims);
        var request = new UpdatePassengerProfileRequest { Name = "Updated", AvatarUrl = "new.jpg" };

        var result = await controller.UpdateCurrentUserProfile(request);

        Assert.IsType<UnauthorizedResult>(result.Result);
    }

    [Fact]
    public async Task UpdateCurrentUserProfile_NoUserRoleClaim_ReturnsUnauthorized()
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, "1")
        };
        SetClaims(claims);
        var request = new UpdatePassengerProfileRequest { Name = "Updated", AvatarUrl = "new.jpg" };

        var result = await controller.UpdateCurrentUserProfile(request);

        Assert.IsType<UnauthorizedResult>(result.Result);
    }

    #endregion
}
