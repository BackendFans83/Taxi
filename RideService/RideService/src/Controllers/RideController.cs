using Microsoft.AspNetCore.Mvc;
using RideService.DTOs;
using RideService.Models;
using RideService.Services;

namespace RideService.Controllers;

[ApiController]
[Route("api/v1/[controller]s")]
public class RideController(IRideService rideService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<RideDto>> CreateRide([FromBody] CreateRideDto dto)
    {
        var result = await rideService.CreateRideAsync(dto);
        return result.IsSuccess ? Ok(result) : StatusCode(result.StatusCode, result.ErrorMessage);
    }
}