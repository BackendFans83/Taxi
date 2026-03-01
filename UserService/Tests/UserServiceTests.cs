using Moq;
using UserService.DTOs;
using UserService.Models;
using UserService.Repositories;

namespace Tests;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockRepository;
    private readonly UserService.Services.UserService _userService;

    public UserServiceTests()
    {
        _mockRepository = new Mock<IUserRepository>();
        _userService = new UserService.Services.UserService(_mockRepository.Object);
    }

    #region CreateUser Tests

    [Fact]
    public async Task CreateUser_WithPassengerRole_CreatesPassengerProfile()
    {
        var createUserDto = new CreateUserDto { Id = 1, Name = "Test User", Role = "Passenger" };
        _mockRepository.Setup(r => r.CreatePassengerProfileAsync(It.IsAny<PassengerProfile>()))
            .ReturnsAsync(true);

        var result = await _userService.CreateUser(createUserDto);

        Assert.True(result.IsSuccess);
        _mockRepository.Verify(r => r.CreatePassengerProfileAsync(It.IsAny<PassengerProfile>()), Times.Once);
    }

    [Fact]
    public async Task CreateUser_WithDriverRole_CreatesDriverProfile()
    {
        var createUserDto = new CreateUserDto { Id = 1, Name = "Test Driver", Role = "Driver" };
        _mockRepository.Setup(r => r.CreateDriverProfileAsync(It.IsAny<DriverProfile>()))
            .ReturnsAsync(true);

        var result = await _userService.CreateUser(createUserDto);

        Assert.True(result.IsSuccess);
        _mockRepository.Verify(r => r.CreateDriverProfileAsync(It.IsAny<DriverProfile>()), Times.Once);
    }

    [Fact]
    public async Task CreateUser_WithAdminRole_CreatesPassengerProfile()
    {
        var createUserDto = new CreateUserDto { Id = 1, Name = "Test Admin", Role = "Admin" };
        _mockRepository.Setup(r => r.CreatePassengerProfileAsync(It.IsAny<PassengerProfile>()))
            .ReturnsAsync(true);

        var result = await _userService.CreateUser(createUserDto);

        Assert.True(result.IsSuccess);
        _mockRepository.Verify(r => r.CreatePassengerProfileAsync(It.IsAny<PassengerProfile>()), Times.Once);
    }

    [Fact]
    public async Task CreateUser_WithInvalidRole_ReturnsFailure()
    {
        var createUserDto = new CreateUserDto { Id = 1, Name = "Test", Role = "InvalidRole" };

        var result = await _userService.CreateUser(createUserDto);

        Assert.False(result.IsSuccess);
        Assert.Equal(400, result.StatusCode);
    }

    [Fact]
    public async Task CreateUser_ProfileAlreadyExists_ReturnsFailure()
    {
        var createUserDto = new CreateUserDto { Id = 1, Name = "Test User", Role = "Passenger" };
        _mockRepository.Setup(r => r.CreatePassengerProfileAsync(It.IsAny<PassengerProfile>()))
            .ReturnsAsync(false);

        var result = await _userService.CreateUser(createUserDto);

        Assert.False(result.IsSuccess);
        Assert.Equal(409, result.StatusCode);
    }

    #endregion

    #region GetPassengerProfileAsync Tests

    [Fact]
    public async Task GetPassengerProfileAsync_ExistingId_ReturnsSuccess()
    {
        var profile = new PassengerProfile(1, "Test Passenger");
        _mockRepository.Setup(r => r.GetPassengerByIdAsync(1))
            .ReturnsAsync(profile);

        var result = await _userService.GetPassengerProfileAsync(1);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Test Passenger", result.Value.Name);
    }

    [Fact]
    public async Task GetPassengerProfileAsync_NonExistingId_ReturnsNotFound()
    {
        _mockRepository.Setup(r => r.GetPassengerByIdAsync(999))
            .ReturnsAsync((PassengerProfile?)null);

        var result = await _userService.GetPassengerProfileAsync(999);

        Assert.False(result.IsSuccess);
        Assert.Equal(404, result.StatusCode);
    }

    #endregion

    #region GetDriverProfileAsync Tests

    [Fact]
    public async Task GetDriverProfileAsync_ExistingId_ReturnsSuccess()
    {
        var profile = new DriverProfile(1, "Test Driver");
        _mockRepository.Setup(r => r.GetDriverByIdAsync(1))
            .ReturnsAsync(profile);

        var result = await _userService.GetDriverProfileAsync(1);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Test Driver", result.Value.Name);
    }

    [Fact]
    public async Task GetDriverProfileAsync_NonExistingId_ReturnsNotFound()
    {
        _mockRepository.Setup(r => r.GetDriverByIdAsync(999))
            .ReturnsAsync((DriverProfile?)null);

        var result = await _userService.GetDriverProfileAsync(999);

        Assert.False(result.IsSuccess);
        Assert.Equal(404, result.StatusCode);
    }

    #endregion

    #region UpdatePassengerProfileAsync Tests

    [Fact]
    public async Task UpdatePassengerProfileAsync_ExistingId_UpdatesProfile()
    {
        var profile = new PassengerProfile(1, "Original Name");
        _mockRepository.Setup(r => r.GetPassengerByIdAsync(1))
            .ReturnsAsync(profile);

        var updateRequest = new UpdatePassengerProfileRequest { Name = "Updated Name", AvatarUrl = "new.jpg" };

        var result = await _userService.UpdatePassengerProfileAsync(1, updateRequest);

        Assert.True(result.IsSuccess);
        Assert.Equal("Updated Name", result.Value.Name);
        Assert.Equal("new.jpg", result.Value.AvatarUrl);
        _mockRepository.Verify(r => r.UpdatePassengerAsync(profile), Times.Once);
    }

    [Fact]
    public async Task UpdatePassengerProfileAsync_NonExistingId_ReturnsNotFound()
    {
        _mockRepository.Setup(r => r.GetPassengerByIdAsync(999))
            .ReturnsAsync((PassengerProfile?)null);

        var updateRequest = new UpdatePassengerProfileRequest { Name = "Updated", AvatarUrl = "new.jpg" };

        var result = await _userService.UpdatePassengerProfileAsync(999, updateRequest);

        Assert.False(result.IsSuccess);
        Assert.Equal(404, result.StatusCode);
    }

    #endregion

    #region UpdateDriverProfileAsync Tests

    [Fact]
    public async Task UpdateDriverProfileAsync_ExistingId_UpdatesProfile()
    {
        var profile = new DriverProfile(1, "Original Name");
        _mockRepository.Setup(r => r.GetDriverByIdAsync(1))
            .ReturnsAsync(profile);

        var updateRequest = new UpdateDriverProfileRequest
        {
            Name = "Updated Driver",
            AvatarUrl = "new.jpg",
            LicenseNumber = "ABC123",
            LicenseExpiryDate = new DateOnly(2027, 1, 1),
            CurrentCarId = 5
        };

        var result = await _userService.UpdateDriverProfileAsync(1, updateRequest);

        Assert.True(result.IsSuccess);
        Assert.Equal("Updated Driver", result.Value.Name);
        Assert.Equal("ABC123", result.Value.LicenseNumber);
        Assert.Equal(5, result.Value.CurrentCarId);
        _mockRepository.Verify(r => r.UpdateDriverAsync(profile), Times.Once);
    }

    [Fact]
    public async Task UpdateDriverProfileAsync_NonExistingId_ReturnsNotFound()
    {
        _mockRepository.Setup(r => r.GetDriverByIdAsync(999))
            .ReturnsAsync((DriverProfile?)null);

        var updateRequest = new UpdateDriverProfileRequest
        {
            Name = "Updated",
            AvatarUrl = "new.jpg",
            LicenseExpiryDate = new DateOnly(2027, 1, 1),
            CurrentCarId = 5
        };

        var result = await _userService.UpdateDriverProfileAsync(999, updateRequest);

        Assert.False(result.IsSuccess);
        Assert.Equal(404, result.StatusCode);
    }

    #endregion
}
