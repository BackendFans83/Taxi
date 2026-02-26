using AuthService.Controllers;
using AuthService.DTOs;
using AuthService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.UnitTests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly AuthController _authController;

    public AuthControllerTests()
    {
        _mockAuthService = new Mock<IAuthService>();
        _authController = new AuthController(_mockAuthService.Object)
        { ControllerContext = new ControllerContext() { HttpContext = new DefaultHttpContext() } };
    }

    [Fact]
    public async Task Register_ValidRequest_ReturnsOkResult()
    {
        var registerRequest = new RegisterRequest("John Doe", "john@example.com", "password123", "Passenger");
        const int id = 1;
        var authResponse = new AuthResponse(id, "access_token");
        var result = Result<AuthResponse>.Success(authResponse);

        _mockAuthService.Setup(service => service.Register(registerRequest)).ReturnsAsync(result);
        _mockAuthService.Setup(service => service.GenerateRefreshToken(id)).ReturnsAsync("refresh_token");

        var actionResult = await _authController.Register(registerRequest);
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var returnValue = Assert.IsType<Result<AuthResponse>>(okResult.Value);
        Assert.True(returnValue.IsSuccess);
        Assert.NotNull(returnValue.Value);
    }

    [Fact]
    public async Task Register_InvalidRequest_ReturnsBadRequest()
    {
        var registerRequest = new RegisterRequest("John Doe", "invalid-email", "password123", "Passenger");
        var result = Result<AuthResponse>.Failure(400, "Invalid email format");

        _mockAuthService.Setup(service => service.Register(registerRequest)).ReturnsAsync(result);

        var actionResult = await _authController.Register(registerRequest);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOkResult()
    {
        var loginRequest = new LoginRequest("john@example.com", "password123");
        var authResponse = new AuthResponse(1, "access_token");
        var result = Result<AuthResponse>.Success(authResponse);

        _mockAuthService.Setup(service => service.Login(loginRequest)).ReturnsAsync(result);
        _mockAuthService.Setup(service => service.GenerateRefreshToken(1)).ReturnsAsync("refresh_token");

        var actionResult = await _authController.Login(loginRequest);
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var returnValue = Assert.IsType<Result<AuthResponse>>(okResult.Value);
        Assert.True(returnValue.IsSuccess);
        Assert.NotNull(returnValue.Value);
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        var loginRequest = new LoginRequest("john@example.com", "wrong_password");
        var result = Result<AuthResponse>.Failure(401, "Invalid credentials");

        _mockAuthService.Setup(service => service.Login(loginRequest)).ReturnsAsync(result);

        var actionResult = await _authController.Login(loginRequest);
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult);
        Assert.Equal(401, unauthorizedResult.StatusCode);
    }

    [Fact]
    public async Task Register_DifferentRoles_ReturnsOkResult()
    {
        var roles = new[] { "Passenger", "Rider", "Admin" };

        foreach (var role in roles)
        {
            var registerRequest = new RegisterRequest("Test User", $"test_{role}@example.com", "password123", role);
            var authResponse = new AuthResponse(1, "access_token");
            var result = Result<AuthResponse>.Success(authResponse);

            _mockAuthService.Setup(service => service.Register(registerRequest)).ReturnsAsync(result);
            _mockAuthService.Setup(service => service.GenerateRefreshToken(1)).ReturnsAsync("refresh_token");

            var actionResult = await _authController.Register(registerRequest);
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var returnValue = Assert.IsType<Result<AuthResponse>>(okResult.Value);
            Assert.True(returnValue.IsSuccess);
        }
    }

    [Fact]
    public async Task Login_DifferentRoles_ReturnsOkResult()
    {
        var roles = new[] { "Passenger", "Rider", "Admin" };

        foreach (var role in roles)
        {
            var loginRequest = new LoginRequest($"test_{role}@example.com", "password123");
            var authResponse = new AuthResponse(1, "access_token");
            var result = Result<AuthResponse>.Success(authResponse);

            _mockAuthService.Setup(service => service.Login(loginRequest)).ReturnsAsync(result);
            _mockAuthService.Setup(service => service.GenerateRefreshToken(1)).ReturnsAsync("refresh_token");

            var actionResult = await _authController.Login(loginRequest);
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var returnValue = Assert.IsType<Result<AuthResponse>>(okResult.Value);
            Assert.True(returnValue.IsSuccess);
        }
    }

    [Fact]
    public async Task Register_ServiceReturnsConflict_ReturnsConflictResult()
    {
        var registerRequest = new RegisterRequest("John Doe", "john@example.com", "password123", "Passenger");
        var result = Result<AuthResponse>.Failure(409, "User with this email already exists");

        _mockAuthService.Setup(service => service.Register(registerRequest)).ReturnsAsync(result);

        var actionResult = await _authController.Register(registerRequest);
        var conflictResult = Assert.IsType<ConflictObjectResult>(actionResult);
        Assert.Equal(409, conflictResult.StatusCode);
    }

    [Fact]
    public async Task Login_ServiceReturnsUnauthorized_ReturnsUnauthorizedResult()
    {
        var loginRequest = new LoginRequest("john@example.com", "wrong_password");
        var result = Result<AuthResponse>.Failure(401, "Invalid credentials");

        _mockAuthService.Setup(service => service.Login(loginRequest)).ReturnsAsync(result);

        var actionResult = await _authController.Login(loginRequest);
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult);
        Assert.Equal(401, unauthorizedResult.StatusCode);
    }

    [Fact]
    public async Task Logout_ValidRefreshToken_ReturnsOkResult()
    {
        const string refreshToken = "valid_refresh_token";
        var result = Result.Success();
        _mockAuthService.Setup(service => service.Logout(refreshToken)).ReturnsAsync(result);
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers.Append("Cookie", $"RefreshToken={refreshToken}");
        _authController.ControllerContext.HttpContext = httpContext;

        var actionResult = await _authController.Logout();

        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var returnValue = Assert.IsType<Result>(okResult.Value);
        Assert.True(returnValue.IsSuccess);
    }

    [Fact]
    public async Task Logout_MissingRefreshToken_ReturnsBadRequest()
    {
        var httpContext = new DefaultHttpContext();
        _authController.ControllerContext.HttpContext = httpContext;

        var actionResult = await _authController.Logout();

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        Assert.Equal("Refresh token not found", badRequestResult.Value);
    }

    [Fact]
    public async Task Logout_InvalidRefreshToken_ReturnsUnauthorized()
    {
        const string refreshToken = "invalid_refresh_token";
        var result = Result.Failure(401, "Invalid refresh token");
        _mockAuthService.Setup(service => service.Logout(refreshToken)).ReturnsAsync(result);
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers.Append("Cookie", $"RefreshToken={refreshToken}");
        _authController.ControllerContext.HttpContext = httpContext;

        var actionResult = await _authController.Logout();

        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult);
        Assert.Equal(401, unauthorizedResult.StatusCode);
    }

    [Fact]
    public async Task Logout_ServiceReturnsFailure_ReturnsInternalServerError()
    {
        const string refreshToken = "refresh_token";
        var result = Result.Failure(500, "Failed to logout");
        _mockAuthService.Setup(service => service.Logout(refreshToken)).ReturnsAsync(result);
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers.Append("Cookie", $"RefreshToken={refreshToken}");
        _authController.ControllerContext.HttpContext = httpContext;

        var actionResult = await _authController.Logout();

        var internalErrorResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(500, internalErrorResult.StatusCode);
    }
}