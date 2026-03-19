using Moq;
using RideService;
using RideService.DTOs;
using RideService.Models;
using RideService.Producers;
using RideService.Repositories;
using RideService.Services;

namespace Tests;

public class RideServiceTests
{
    private readonly Mock<IRideRepository> _rideRepositoryMock;
    private readonly Mock<IKafkaProducer> _kafkaProducerMock;
    private readonly RideService.Services.RideService _rideService;

    public RideServiceTests()
    {
        _rideRepositoryMock = new Mock<IRideRepository>();
        _kafkaProducerMock = new Mock<IKafkaProducer>();
        _rideService = new RideService.Services.RideService(_rideRepositoryMock.Object, _kafkaProducerMock.Object);
    }

    [Fact]
    public async Task CreateRideAsync_ValidDto_CallsRepositoryAndReturnsSuccess()
    {
        var dto = new CreateRideDto
        {
            PassengerId = 1,
            PickupLatitude = 83.97,
            PickupLongitude = 83.97,
            DropOffLatitude = 83.83,
            DropOffLongitude = 83.83,
            PickupAddress = "ул. Пушкина",
            DropOffAddress = "ул. Колотушкина"
        };

        var savedRide = new Ride
        {
            Id = 1,
            PassengerId = dto.PassengerId,
            PickupLatitude = dto.PickupLatitude,
            PickupLongitude = dto.PickupLongitude,
            DropOffLatitude = dto.DropOffLatitude,
            DropOffLongitude = dto.DropOffLongitude,
            PickupAddress = dto.PickupAddress,
            DropOffAddress = dto.DropOffAddress,
            Status = RideStatus.Requested,
            RequestedAt = DateTime.UtcNow
        };

        _rideRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Ride>())).ReturnsAsync(savedRide);

        var result = await _rideService.CreateRideAsync(dto);

        Assert.True(result.IsSuccess);
        Assert.Equal(savedRide.Id, result.Value!.Id);
        Assert.Equal(RideStatus.Requested, result.Value.Status);
        Assert.Equal(dto.PassengerId, result.Value.PassengerId);
        Assert.Equal(dto.PickupLatitude, result.Value.PickupLatitude);
        Assert.Equal(dto.DropOffLongitude, result.Value.DropOffLongitude);

        _rideRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Ride>()), Times.Once);
    }

    [Fact]
    public async Task CreateRideAsync_SetsRequestedStatus()
    {
        var dto = new CreateRideDto
        {
            PassengerId = 1,
            PickupLatitude = 83.97,
            PickupLongitude = 83.97,
            DropOffLatitude = 83.83,
            DropOffLongitude = 83.83,
            PickupAddress = "ул. Пушкина",
            DropOffAddress = "ул. Колотушкина"
        };

        Ride? capturedRide = null;
        _rideRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Ride>()))
            .Callback<Ride>(r => capturedRide = r)
            .ReturnsAsync(new Ride { Id = 1 });

        await _rideService.CreateRideAsync(dto);

        Assert.NotNull(capturedRide);
        Assert.Equal(RideStatus.Requested, capturedRide.Status);
        Assert.True(capturedRide.RequestedAt <= DateTime.UtcNow);
    }

    [Fact]
    public async Task CreateRideAsync_CallsKafkaProducer_OnSuccess()
    {
        var dto = new CreateRideDto
        {
            PassengerId = 1,
            PickupLatitude = 83.97,
            PickupLongitude = 83.97,
            DropOffLatitude = 83.83,
            DropOffLongitude = 83.83,
            PickupAddress = "ул. Пушкина",
            DropOffAddress = "ул. Колотушкина"
        };

        var savedRide = new Ride
        {
            Id = 1,
            PassengerId = dto.PassengerId,
            PickupLatitude = dto.PickupLatitude,
            PickupLongitude = dto.PickupLongitude,
            DropOffLatitude = dto.DropOffLatitude,
            DropOffLongitude = dto.DropOffLongitude,
            PickupAddress = dto.PickupAddress,
            DropOffAddress = dto.DropOffAddress,
            Status = RideStatus.Requested
        };

        _rideRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Ride>())).ReturnsAsync(savedRide);
        _kafkaProducerMock.Setup(p => p.SendRideCreatedEventAsync(It.IsAny<RideDto>()))
            .ReturnsAsync(Result.Success());

        await _rideService.CreateRideAsync(dto);

        _kafkaProducerMock.Verify(p => p.SendRideCreatedEventAsync(It.IsAny<RideDto>()), Times.Once);
    }

    [Fact]
    public async Task CreateRideAsync_RepositoryThrows_ReturnsFailure()
    {
        var dto = new CreateRideDto
        {
            PassengerId = 1,
            PickupLatitude = 83.97,
            PickupLongitude = 83.97,
            DropOffLatitude = 83.83,
            DropOffLongitude = 83.83,
            PickupAddress = "ул. Пушкина",
            DropOffAddress = "ул. Колотушкина"
        };

        _rideRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Ride>()))
            .ThrowsAsync(new Exception("DB error"));

        var result = await _rideService.CreateRideAsync(dto);

        Assert.False(result.IsSuccess);
        Assert.Equal(500, result.StatusCode);
        Assert.Contains("DB error", result.ErrorMessage);
    }
}
