using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using UserService.Attributes;
using UserService.DTOs;
using UserService.Enums;
using UserService.Services;

namespace UserService.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpGet("me")]
    [ProducesResponseType(typeof(PassengerProfileDto),200)]
    [SwaggerOneOfResponse(typeof(PassengerProfileDto), typeof(DriverProfileDto))]
    public async Task<ActionResult<object>> GetCurrentUserProfile()
    {
        var userId = GetUserIdFromClaims();
        var role = GetRoleFromClaims();

        if (userId == null || role == null)
            return Unauthorized();

        switch (role)
        {
            case Role.Passenger:
            case Role.Admin:
                var passengerResult = await userService.GetPassengerProfileAsync(userId.Value);
                return passengerResult.IsSuccess
                    ? Ok(passengerResult.Value)
                    : StatusCode(passengerResult.StatusCode, passengerResult.ErrorMessage);

            case Role.Driver:
                var driverResult = await userService.GetDriverProfileAsync(userId.Value);
                return driverResult.IsSuccess
                    ? Ok(driverResult.Value)
                    : StatusCode(driverResult.StatusCode, driverResult.ErrorMessage);

            default:
                return Unauthorized();
        }
    }
    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PassengerProfileDto>> GetPassengerProfile(int id)
    {
        var passengerResult = await userService.GetPassengerProfileAsync(id);
        return passengerResult.IsSuccess
            ? Ok(passengerResult.Value)
            : StatusCode(passengerResult.StatusCode, passengerResult.ErrorMessage);
    }

    [HttpPut("me")]
    [ProducesResponseType(typeof(PassengerProfileDto),200)]
    [SwaggerOneOfRequest(typeof(UpdatePassengerProfileRequest), typeof(UpdateDriverProfileRequest))]
    [SwaggerOneOfResponse(typeof(PassengerProfileDto), typeof(DriverProfileDto))]
    public async Task<ActionResult<object>> UpdateCurrentUserProfile([FromBody] object request)
    {
        var userId = GetUserIdFromClaims();
        var role = GetRoleFromClaims();

        if (userId == null || role == null)
            return Unauthorized();

        switch (role)
        {
            case Role.Passenger:
            case Role.Admin:
                if (request is not UpdatePassengerProfileRequest passengerRequest)
                    return BadRequest();
                var passengerResult = await userService.UpdatePassengerProfileAsync(userId.Value, passengerRequest);
                return passengerResult.IsSuccess
                    ? Ok(passengerResult.Value)
                    : StatusCode(passengerResult.StatusCode, passengerResult.ErrorMessage);

            case Role.Driver:
                if (request is not UpdateDriverProfileRequest driverRequest)
                    return BadRequest();
                var driverResult = await userService.UpdateDriverProfileAsync(userId.Value, driverRequest);
                return driverResult.IsSuccess
                    ? Ok(driverResult.Value)
                    : StatusCode(driverResult.StatusCode, driverResult.ErrorMessage);

            default:
                return Unauthorized();
        }
    }

    private int? GetUserIdFromClaims()
    {
        var claim = User.FindFirst(JwtRegisteredClaimNames.Sub);
        if (claim == null || !int.TryParse(claim.Value, out var userId))
            return null;
        return userId;
    }

    private Role? GetRoleFromClaims()
    {
        var claim = User.FindFirst(ClaimTypes.Role);
        if (claim == null || !Enum.TryParse<Role>(claim.Value, ignoreCase: true, out var role))
            return null;
        return role;
    }
}