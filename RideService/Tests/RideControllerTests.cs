using Microsoft.AspNetCore.Mvc;
using Moq;
using RideService;
using RideService.Controllers;
using RideService.DTOs;
using RideService.Models;
using RideService.Services;

namespace Tests;

public class RideControllerTests
{
    private readonly Mock<IRideService> _rideServiceMock;
    private readonly RideController _controller;

    public RideControllerTests()
    {
        _rideServiceMock = new Mock<IRideService>();
        _controller = new RideController(_rideServiceMock.Object);
    }

    [Fact]
    public async Task CreateRide_ValidDto_ReturnsOkWithRide()
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

        var ride = new Ride
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

        var result = Result<Ride>.Success(ride);
        _rideServiceMock.Setup(s => s.CreateRideAsync(dto)).ReturnsAsync(result);

        var actionResult = await _controller.CreateRide(dto);
        var okResult = Assert.IsType<ActionResult<Ride>>(actionResult);
        var objectResult = Assert.IsType<OkObjectResult>(okResult.Result);
        var returnedResult = Assert.IsType<Result<Ride>>(objectResult.Value);

        Assert.True(returnedResult.IsSuccess);
        Assert.Equal(ride.Id, returnedResult.Value!.Id);
        Assert.Equal(RideStatus.Requested, returnedResult.Value.Status);
    }

    [Fact]
    public async Task CreateRide_ServiceReturnsFailure_ReturnsStatusCodeWithError()
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

        var result = Result<Ride>.Failure(500, "Database error");
        _rideServiceMock.Setup(s => s.CreateRideAsync(dto)).ReturnsAsync(result);

        var actionResult = await _controller.CreateRide(dto);
        var statusCodeResult = Assert.IsType<ActionResult<Ride>>(actionResult);
        var objectResult = Assert.IsType<ObjectResult>(statusCodeResult.Result);

        Assert.Equal(500, objectResult.StatusCode);
        Assert.Equal("Database error", objectResult.Value);
    }
}
