using AuthService.DTOs;
using AuthService.Enums;
using AuthService.Models;
using AuthService.Repositories;
using AuthService.Utils;
using Moq;

namespace Tests.UnitTests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IAuthRepository> _mockAuthRepository;
        private readonly Mock<ICacheRepository> _mockCacheRepository;
        private readonly Mock<IAccessTokenGenerator> _mockAccessTokenGenerator;
        private readonly Mock<IRefreshTokenGenerator> _mockRefreshTokenGenerator;
        private readonly AuthService.Services.AuthService _authService;

        public AuthServiceTests()
        {
            _mockAuthRepository = new Mock<IAuthRepository>();
            _mockCacheRepository = new Mock<ICacheRepository>();
            _mockAccessTokenGenerator = new Mock<IAccessTokenGenerator>();
            _mockRefreshTokenGenerator = new Mock<IRefreshTokenGenerator>();
            _authService = new AuthService.Services.AuthService(
                _mockAuthRepository.Object,
                _mockCacheRepository.Object,
                _mockAccessTokenGenerator.Object,
                _mockRefreshTokenGenerator.Object
            );
        }

        [Fact]
        public async Task Register_ValidRequest_CreatesUserSuccessfully()
        {
            var registerRequest = new RegisterRequest("John Doe", "john@example.com", "password123", "Passenger");
            const string accessToken = "access_token";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password);
            const int userId = 1;
            _mockAuthRepository.SetupSequence(x => x.GetUserCredentialsByEmail(registerRequest.Email))
                .ReturnsAsync((Credentials?)null)
                .ReturnsAsync(new Credentials(userId, "john@example.com", hashedPassword, Role.Passenger));
            _mockAuthRepository.Setup(x => x.CreateUserCredentials(It.IsAny<Credentials>()))
                .ReturnsAsync(true);
            _mockAccessTokenGenerator.Setup(x => x.GenerateAccessToken(userId, Role.Passenger))
                .Returns(accessToken);

            var result = await _authService.Register(registerRequest);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(accessToken, result.Value.AccessToken);
            _mockAuthRepository.Verify(x => x.CreateUserCredentials(It.IsAny<Credentials>()), Times.Once);
        }

        [Fact]
        public async Task Register_UserAlreadyExists_ReturnsFailure()
        {
            var registerRequest = new RegisterRequest("John Doe", "john@example.com", "password123", "Passenger");
            var existingUser = new Credentials(1, "john@example.com", "existing_hash", Role.Passenger);

            _mockAuthRepository.Setup(x => x.GetUserCredentialsByEmail(registerRequest.Email))
                .ReturnsAsync(existingUser);

            var result = await _authService.Register(registerRequest);

            Assert.False(result.IsSuccess);
            Assert.Null(result.Value);
            _mockAuthRepository.Verify(x => x.CreateUserCredentials(It.IsAny<Credentials>()), Times.Never);
        }

        [Fact]
        public async Task Register_InvalidRole_ReturnsFailure()
        {
            var registerRequest = new RegisterRequest("John Doe", "john@example.com", "password123", "InvalidRole");

            var result = await _authService.Register(registerRequest);

            Assert.False(result.IsSuccess);
            Assert.Null(result.Value);
        }

        [Fact]
        public async Task Register_CreateUserFails_ReturnsFailure()
        {
            var registerRequest = new RegisterRequest("John Doe", "john@example.com", "password123", "Passenger");

            _mockAuthRepository.Setup(x => x.GetUserCredentialsByEmail(registerRequest.Email))
                .ReturnsAsync((Credentials)null);
            _mockAuthRepository.Setup(x => x.CreateUserCredentials(It.IsAny<Credentials>()))
                .ReturnsAsync(false);

            var result = await _authService.Register(registerRequest);

            Assert.False(result.IsSuccess);
            Assert.Null(result.Value);
        }

        [Fact]
        public async Task Register_GetSavedUserFails_ReturnsFailure()
        {
            var registerRequest = new RegisterRequest("John Doe", "john@example.com", "password123", "Passenger");

            _mockAuthRepository.Setup(x => x.GetUserCredentialsByEmail(registerRequest.Email))
                .ReturnsAsync((Credentials)null);
            _mockAuthRepository.Setup(x => x.CreateUserCredentials(It.IsAny<Credentials>()))
                .ReturnsAsync(true);
            _mockAuthRepository.Setup(x => x.GetUserCredentialsByEmail(registerRequest.Email))
                .ReturnsAsync((Credentials)null);

            var result = await _authService.Register(registerRequest);

            Assert.False(result.IsSuccess);
            Assert.Null(result.Value);
        }

        [Fact]
        public async Task Register_StoreRefreshTokenFails_ReturnsFailure()
        {
            var registerRequest = new RegisterRequest("John Doe", "john@example.com", "password123", "Passenger");
            var userId = 1;
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password);

            _mockAuthRepository.Setup(x => x.GetUserCredentialsByEmail(registerRequest.Email))
                .ReturnsAsync((Credentials)null);
            _mockAuthRepository.Setup(x => x.CreateUserCredentials(It.IsAny<Credentials>()))
                .ReturnsAsync(true);
            _mockAuthRepository.Setup(x => x.GetUserCredentialsByEmail(registerRequest.Email))
                .ReturnsAsync(new Credentials(userId, "john@example.com", hashedPassword, Role.Passenger));
            _mockCacheRepository.Setup(x => x.AddRefreshToken(userId, It.IsAny<string>()))
                .ReturnsAsync(false);

            var result = await _authService.Register(registerRequest);

            Assert.False(result.IsSuccess);
            Assert.Null(result.Value);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsSuccess()
        {
            var loginRequest = new LoginRequest("john@example.com", "password123");
            const string accessToken = "access_token";
            const int userId = 1;
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(loginRequest.Password);
            var credentials = new Credentials(userId, loginRequest.Email, hashedPassword, Role.Passenger);

            _mockAuthRepository.Setup(x => x.GetUserCredentialsByEmail(loginRequest.Email))
                .ReturnsAsync(credentials);
            _mockAccessTokenGenerator.Setup(x => x.GenerateAccessToken(userId, Role.Passenger))
                .Returns(accessToken);

            var result = await _authService.Login(loginRequest);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(userId, result.Value.Id);
            Assert.Equal(accessToken, result.Value.AccessToken);
        }

        [Fact]
        public async Task Login_UserNotFound_ReturnsFailure()
        {
            var loginRequest = new LoginRequest("nonexistent@example.com", "password123");

            _mockAuthRepository.Setup(x => x.GetUserCredentialsByEmail(loginRequest.Email))
                .ReturnsAsync((Credentials?)null);

            var result = await _authService.Login(loginRequest);

            Assert.False(result.IsSuccess);
            Assert.Null(result.Value);
        }

        [Fact]
        public async Task Login_InvalidPassword_ReturnsFailure()
        {
            var loginRequest = new LoginRequest("john@example.com", "wrongpassword");
            var correctPassword = "password123";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(correctPassword);
            var credentials = new Credentials(1, loginRequest.Email, hashedPassword, Role.Passenger);

            _mockAuthRepository.Setup(x => x.GetUserCredentialsByEmail(loginRequest.Email))
                .ReturnsAsync(credentials);

            var result = await _authService.Login(loginRequest);

            Assert.False(result.IsSuccess);
            Assert.Null(result.Value);
        }

        [Fact]
        public async Task Login_EmailCaseInsensitive()
        {
            var loginRequest = new LoginRequest("JOHN@EXAMPLE.COM", "password123");
            const string accessToken = "access_token";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(loginRequest.Password);
            var credentials = new Credentials(1, "john@example.com", hashedPassword, Role.Passenger);

            _mockAuthRepository.Setup(x => x.GetUserCredentialsByEmail("john@example.com"))
                .ReturnsAsync(credentials);
            _mockAccessTokenGenerator.Setup(x => x.GenerateAccessToken(1, Role.Passenger))
                .Returns(accessToken);

            var result = await _authService.Login(loginRequest);

            Assert.True(result.IsSuccess);
            _mockAuthRepository.Verify(x => x.GetUserCredentialsByEmail("john@example.com"), Times.Once);
        }

        [Fact]
        public async Task Login_DifferentRoles_ReturnsSuccess()
        {
            const string accessToken = "access_token";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("password123");

            var roles = new[] { Role.Passenger, Role.Driver, Role.Admin };

            foreach (var role in roles)
            {
                var loginRequest = new LoginRequest($"{role}@example.com".ToLower(), "password123");
                var credentials = new Credentials(1, loginRequest.Email, hashedPassword, role);

                _mockAuthRepository.Setup(x => x.GetUserCredentialsByEmail(loginRequest.Email))
                    .ReturnsAsync(credentials);
                _mockAccessTokenGenerator.Setup(x => x.GenerateAccessToken(1, role))
                    .Returns(accessToken);

                var result = await _authService.Login(loginRequest);

                Assert.True(result.IsSuccess, result.ErrorMessage);
                Assert.NotNull(result.Value);
                Assert.Equal(role, credentials.Role);
            }
        }

        [Fact]
        public async Task GenerateRefreshToken_ValidId_ReturnsToken()
        {
            const int userId = 1;
            const string refreshToken = "generated_refresh_token";

            _mockRefreshTokenGenerator.Setup(x => x.GenerateRefreshToken())
                .Returns(refreshToken);
            _mockCacheRepository.Setup(x => x.AddRefreshToken(userId, refreshToken))
                .ReturnsAsync(true);

            var result = await _authService.GenerateRefreshToken(userId);

            Assert.NotNull(result);
            Assert.Equal(refreshToken, result);
        }

        [Fact]
        public async Task GenerateRefreshToken_CacheFails_ReturnsNull()
        {
            const int userId = 1;
            const string refreshToken = "generated_refresh_token";

            _mockRefreshTokenGenerator.Setup(x => x.GenerateRefreshToken())
                .Returns(refreshToken);
            _mockCacheRepository.Setup(x => x.AddRefreshToken(userId, refreshToken))
                .ReturnsAsync(false);

            var result = await _authService.GenerateRefreshToken(userId);

            Assert.Null(result);
        }

        [Fact]
        public async Task Register_DifferentRoles_ReturnsSuccess()
        {
            const string accessToken = "access_token";
            var roles = new[] { "Passenger", "Driver", "Admin" };

            foreach (var roleStr in roles)
            {
                var registerRequest = new RegisterRequest("Test User", $"test_{roleStr}@example.com".ToLower(),
                    "password123", roleStr);
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password);
                const int userId = 1;

                _mockAuthRepository.SetupSequence(x => x.GetUserCredentialsByEmail(registerRequest.Email))
                    .ReturnsAsync((Credentials?)null)
                    .ReturnsAsync(new Credentials(userId, registerRequest.Email, hashedPassword,
                        Enum.Parse<Role>(roleStr)));
                _mockAuthRepository.Setup(x => x.CreateUserCredentials(It.IsAny<Credentials>()))
                    .ReturnsAsync(true);
                _mockAccessTokenGenerator.Setup(x => x.GenerateAccessToken(userId, Enum.Parse<Role>(roleStr)))
                    .Returns(accessToken);

                var result = await _authService.Register(registerRequest);

                Assert.True(result.IsSuccess);
                Assert.NotNull(result.Value);
                Assert.Equal(accessToken, result.Value.AccessToken);
            }
        }

        [Fact]
        public async Task Logout_ValidRefreshToken_ReturnsSuccess()
        {
            const string refreshToken = "valid_refresh_token";
            const int userId = 1;
            _mockCacheRepository.Setup(x => x.GetUserIdByRefreshToken(refreshToken))
                .ReturnsAsync(userId);
            _mockCacheRepository.Setup(x => x.DeleteRefreshToken(refreshToken))
                .ReturnsAsync(true);

            var result = await _authService.Logout(refreshToken);

            Assert.True(result.IsSuccess);
            Assert.Equal(200, result.StatusCode);
            _mockCacheRepository.Verify(x => x.DeleteRefreshToken(refreshToken), Times.Once);
        }

        [Fact]
        public async Task Logout_InvalidRefreshToken_ReturnsUnauthorized()
        {
            const string refreshToken = "invalid_refresh_token";
            _mockCacheRepository.Setup(x => x.GetUserIdByRefreshToken(refreshToken)).ReturnsAsync((int?)null);

            var result = await _authService.Logout(refreshToken);

            Assert.False(result.IsSuccess);
            Assert.Equal(401, result.StatusCode);
            Assert.Equal("Invalid refresh token", result.ErrorMessage);
            _mockCacheRepository.Verify(x => x.DeleteRefreshToken(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Logout_DeleteRefreshTokenFails_ReturnsFailure()
        {
            const string refreshToken = "refresh_token";
            const int userId = 1;
            _mockCacheRepository.Setup(x => x.GetUserIdByRefreshToken(refreshToken)).ReturnsAsync(userId);
            _mockCacheRepository.Setup(x => x.DeleteRefreshToken(refreshToken)).ReturnsAsync(false);

            var result = await _authService.Logout(refreshToken);

            Assert.False(result.IsSuccess);
            Assert.Equal(500, result.StatusCode);
            Assert.Equal("Failed to logout", result.ErrorMessage);
        }
    }
}