using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using OnlineStore.API.Controllers;
using OnlineStore.Services.Services;
using OnlineStore.Core;
using OnlineStore.Core.DTOs;
using OnlineStore.Core.Interfaces;
using OnlineStore.Core.Models;
using OnlineStore.Core.Services;
using FluentValidation;
using Xunit;
using FluentAssertions;

namespace OnlineStore.Tests.Authorization
{
    public class AutorizationTests
    {
        private readonly Mock<OnlineStoreDbContext> _mockContext;
        private readonly Mock<IValidator<RegisterDTO>> _mockRegisterValidator;
        private readonly Mock<IValidator<ForgotPasswordDTO>> _mockForgotPasswordValidator;
        private readonly Mock<IValidator<ResetPasswordDTO>> _mockResetPasswordValidator;
        private readonly Mock<IValidator<ChangePasswordDTO>> _mockChangePasswordValidator;
        private readonly Mock<IValidator<ConfirmEmailDTO>> _mockConfirmEmailValidator;
        private readonly Mock<IJwtService> _mockJwtService;
        private readonly Mock<IAuditService> _mockAuditService;
        private readonly AuthController _controller;

        public AutorizationTests()
        {
            _mockContext = new Mock<OnlineStoreDbContext>(new DbContextOptions<OnlineStoreDbContext>());
            _mockRegisterValidator = new Mock<IValidator<RegisterDTO>>();
            _mockForgotPasswordValidator = new Mock<IValidator<ForgotPasswordDTO>>();
            _mockResetPasswordValidator = new Mock<IValidator<ResetPasswordDTO>>();
            _mockChangePasswordValidator = new Mock<IValidator<ChangePasswordDTO>>();
            _mockConfirmEmailValidator = new Mock<IValidator<ConfirmEmailDTO>>();
            _mockJwtService = new Mock<IJwtService>();
            _mockAuditService = new Mock<IAuditService>();

            // Setup empty DbSets
            SetupEmptyDbSets();

            _controller = new AuthController(
                _mockContext.Object,
                _mockRegisterValidator.Object,
                _mockForgotPasswordValidator.Object,
                _mockResetPasswordValidator.Object,
                _mockChangePasswordValidator.Object,
                _mockConfirmEmailValidator.Object,
                _mockJwtService.Object,
                _mockAuditService.Object
            );

            // Setup HttpContext
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["User-Agent"] = "Test-Agent";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        private void SetupEmptyDbSets()
        {
            // Setup empty DbSet objects to avoid NullReferenceException
            _mockContext.Setup(c => c.Users).Returns(CreateMockDbSet(new List<User>().AsQueryable()).Object);
            _mockContext.Setup(c => c.Roles).Returns(CreateMockDbSet(new List<Role>().AsQueryable()).Object);
            _mockContext.Setup(c => c.UserRoles).Returns(CreateMockDbSet(new List<UserRole>().AsQueryable()).Object);
            _mockContext.Setup(c => c.UserSessions).Returns(CreateMockDbSet(new List<UserSession>().AsQueryable()).Object);
        }

        #region Login Tests

        [Fact]
        public async Task Login_ValidEmailAndPassword_ReturnsOkResult()
        {
            // Arrange
            var dto = new LoginDTO
            {
                EmailOrUsername = "test@example.com",
                Password = "Password123!"
            };

            var user = new User
            {
                Id = 1,
                Email = "test@example.com",
                Username = "testuser",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                IsActive = true,
                IsEmailConfirmed = true,
                IsDeleted = false,
                UserRoles = new List<UserRole>()
            };

            var mockUsersDbSet = CreateMockDbSet(new List<User> { user }.AsQueryable());
            _mockContext.Setup(c => c.Users).Returns(mockUsersDbSet.Object);

            _mockJwtService.Setup(j => j.GenerateAccessToken(It.IsAny<User>(), It.IsAny<List<string>>()))
                .Returns("access_token");
            _mockJwtService.Setup(j => j.GenerateRefreshToken()).Returns("refresh_token");

            // Act
            var result = await _controller.Login(dto);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Login_ValidUsernameAndPassword_ReturnsOkResult()
        {
            // Arrange
            var dto = new LoginDTO
            {
                EmailOrUsername = "testuser",
                Password = "Password123!"
            };

            var user = new User
            {
                Id = 1,
                Email = "test@example.com",
                Username = "testuser",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                IsActive = true,
                IsEmailConfirmed = true,
                IsDeleted = false,
                UserRoles = new List<UserRole>()
            };

            var mockUsersDbSet = CreateMockDbSet(new List<User> { user }.AsQueryable());
            _mockContext.Setup(c => c.Users).Returns(mockUsersDbSet.Object);

            _mockJwtService.Setup(j => j.GenerateAccessToken(It.IsAny<User>(), It.IsAny<List<string>>()))
                .Returns("access_token");
            _mockJwtService.Setup(j => j.GenerateRefreshToken()).Returns("refresh_token");

            // Act
            var result = await _controller.Login(dto);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Login_InvalidPassword_ReturnsUnauthorized()
        {
            // Arrange
            var dto = new LoginDTO
            {
                EmailOrUsername = "test@example.com",
                Password = "WrongPassword"
            };

            var user = new User
            {
                Id = 1,
                Email = "test@example.com",
                Username = "testuser",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                IsActive = true,
                IsEmailConfirmed = true,
                IsDeleted = false,
                UserRoles = new List<UserRole>()
            };

            var mockUsersDbSet = CreateMockDbSet(new List<User> { user }.AsQueryable());
            _mockContext.Setup(c => c.Users).Returns(mockUsersDbSet.Object);

            // Act
            var result = await _controller.Login(dto);

            // Assert
            result.Result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task Login_NonExistentUser_ReturnsUnauthorized()
        {
            // Arrange
            var dto = new LoginDTO
            {
                EmailOrUsername = "nonexistent@example.com",
                Password = "Password123!"
            };

            var mockUsersDbSet = CreateMockDbSet(new List<User>().AsQueryable());
            _mockContext.Setup(c => c.Users).Returns(mockUsersDbSet.Object);

            // Act
            var result = await _controller.Login(dto);

            // Assert
            result.Result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task Login_InactiveUser_ReturnsUnauthorized()
        {
            // Arrange
            var dto = new LoginDTO
            {
                EmailOrUsername = "test@example.com",
                Password = "Password123!"
            };

            var user = new User
            {
                Id = 1,
                Email = "test@example.com",
                Username = "testuser",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                IsActive = false,
                IsEmailConfirmed = true,
                IsDeleted = false,
                UserRoles = new List<UserRole>()
            };

            var mockUsersDbSet = CreateMockDbSet(new List<User> { user }.AsQueryable());
            _mockContext.Setup(c => c.Users).Returns(mockUsersDbSet.Object);
            
            _mockJwtService.Setup(j => j.GenerateRefreshToken()).Returns("refresh_token");

            // Act
            var result = await _controller.Login(dto);

            // Assert
            result.Result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        #endregion

        #region Refresh Tests

        [Fact]
        public async Task Refresh_ValidTokens_ReturnsOkResult()
        {
            // Arrange
            var dto = new RefreshTokenRequestDTO
            {
                AccessToken = "expired_access_token",
                RefreshToken = "valid_refresh_token"
            };

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1")
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);

            var user = new User
            {
                Id = 1,
                Email = "test@example.com",
                Username = "testuser",
                UserRoles = new List<UserRole>()
            };

            var userSession = new UserSession
            {
                UserId = 1,
                RefreshTokenHash = BCrypt.Net.BCrypt.HashPassword("valid_refresh_token"),
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                IsRevoked = false
            };

            _mockJwtService.Setup(j => j.GetPrincipalFromExpiredToken(dto.AccessToken)).Returns(principal);
            _mockContext.Setup(c => c.Users).Returns(CreateMockDbSet(new List<User> { user }.AsQueryable()).Object);
            _mockContext.Setup(c => c.UserSessions).Returns(CreateMockDbSet(new List<UserSession> { userSession }.AsQueryable()).Object);
            _mockJwtService.Setup(j => j.GenerateAccessToken(It.IsAny<User>(), It.IsAny<List<string>>()))
                .Returns("new_access_token");
            _mockJwtService.Setup(j => j.GenerateRefreshToken()).Returns("new_refresh_token");

            // Act
            var result = await _controller.Refresh(dto);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Refresh_InvalidRefreshToken_ReturnsUnauthorized()
        {
            // Arrange
            var dto = new RefreshTokenRequestDTO
            {
                AccessToken = "expired_access_token",
                RefreshToken = "invalid_refresh_token"
            };

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1")
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);

            var user = new User
            {
                Id = 1,
                Email = "test@example.com",
                Username = "testuser",
                UserRoles = new List<UserRole>()
            };

            var userSession = new UserSession
            {
                UserId = 1,
                RefreshTokenHash = BCrypt.Net.BCrypt.HashPassword("valid_refresh_token"),
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                IsRevoked = false
            };

            _mockJwtService.Setup(j => j.GetPrincipalFromExpiredToken(dto.AccessToken)).Returns(principal);
            _mockContext.Setup(c => c.Users).Returns(CreateMockDbSet(new List<User> { user }.AsQueryable()).Object);
            _mockContext.Setup(c => c.UserSessions).Returns(CreateMockDbSet(new List<UserSession> { userSession }.AsQueryable()).Object);

            // Act
            var result = await _controller.Refresh(dto);

            // Assert
            result.Result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task Refresh_ExpiredRefreshToken_ReturnsUnauthorized()
        {
            // Arrange
            var dto = new RefreshTokenRequestDTO
            {
                AccessToken = "expired_access_token",
                RefreshToken = "valid_refresh_token"
            };

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1")
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);

            var user = new User
            {
                Id = 1,
                Email = "test@example.com",
                Username = "testuser",
                UserRoles = new List<UserRole>()
            };

            var userSession = new UserSession
            {
                UserId = 1,
                RefreshTokenHash = BCrypt.Net.BCrypt.HashPassword("valid_refresh_token"),
                ExpiresAt = DateTime.UtcNow.AddDays(-1), // Истекший токен
                IsRevoked = false
            };

            _mockJwtService.Setup(j => j.GetPrincipalFromExpiredToken(dto.AccessToken)).Returns(principal);
            _mockContext.Setup(c => c.Users).Returns(CreateMockDbSet(new List<User> { user }.AsQueryable()).Object);
            _mockContext.Setup(c => c.UserSessions).Returns(CreateMockDbSet(new List<UserSession> { userSession }.AsQueryable()).Object);

            // Act
            var result = await _controller.Refresh(dto);

            // Assert
            result.Result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        #endregion

        #region Logout Tests

        [Fact]
        public async Task Logout_AuthenticatedUser_ReturnsOkResult()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Email, "test@example.com")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            var userSessions = new List<UserSession>
            {
                new UserSession { UserId = 1, IsRevoked = false }
            };

            _mockContext.Setup(c => c.UserSessions).Returns(CreateMockDbSet(userSessions.AsQueryable()).Object);

            // Act
            var result = await _controller.Logout();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        #endregion

        #region Revoke Tests

        [Fact]
        public async Task Revoke_AuthenticatedUser_ReturnsOkResult()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Email, "test@example.com")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            var userSessions = new List<UserSession>
            {
                new UserSession { UserId = 1, IsRevoked = false }
            };

            _mockContext.Setup(c => c.UserSessions).Returns(CreateMockDbSet(userSessions.AsQueryable()).Object);

            // Act
            var result = await _controller.Revoke();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        #endregion

        #region ForgotPassword Tests

        [Fact]
        public async Task ForgotPassword_ValidEmail_ReturnsOkResult()
        {
            // Arrange
            var dto = new ForgotPasswordDTO { Email = "test@example.com" };

            var user = new User
            {
                Id = 1,
                Email = "test@example.com",
                IsActive = true,
                IsDeleted = false,
                IsEmailConfirmed = true
            };

            _mockForgotPasswordValidator.Setup(v => v.ValidateAsync(dto, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _mockContext.Setup(c => c.Users).Returns(CreateMockDbSet(new List<User> { user }.AsQueryable()).Object);

            // Act
            var result = await _controller.ForgotPassword(dto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task ForgotPassword_NonExistentEmail_ReturnsOkResult()
        {
            // Arrange
            var dto = new ForgotPasswordDTO { Email = "nonexistent@example.com" };

            _mockForgotPasswordValidator.Setup(v => v.ValidateAsync(dto, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _mockContext.Setup(c => c.Users).Returns(CreateMockDbSet(new List<User>().AsQueryable()).Object);

            // Act
            var result = await _controller.ForgotPassword(dto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        #endregion

        #region ResetPassword Tests

        [Fact]
        public async Task ResetPassword_ValidToken_ReturnsOkResult()
        {
            // Arrange
            var dto = new ResetPasswordDTO
            {
                Token = "valid_token",
                NewPassword = "NewPassword123!"
            };

            var user = new User
            {
                Id = 1,
                ResetTokenHash = BCrypt.Net.BCrypt.HashPassword("valid_token"),
                ResetTokenExpiry = DateTime.UtcNow.AddHours(1)
            };

            var userRoles = new List<UserRole>
            {
                new UserRole
                {
                    UserId = 1,
                    RoleId = 1,
                    Role = new Role { Id = 1, Name = "Пользователь" }
                }
            };

            _mockResetPasswordValidator.Setup(v => v.ValidateAsync(dto, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _mockContext.Setup(c => c.Users).Returns(CreateMockDbSet(new List<User> { user }.AsQueryable()).Object);
            _mockContext.Setup(c => c.UserRoles).Returns(CreateMockDbSet(userRoles.AsQueryable()).Object);
            _mockJwtService.Setup(j => j.GenerateAccessToken(It.IsAny<User>(), It.IsAny<List<string>>()))
                .Returns("access_token");
            _mockJwtService.Setup(j => j.GenerateRefreshToken()).Returns("refresh_token");

            // Act
            var result = await _controller.ResetPassword(dto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task ResetPassword_InvalidToken_ReturnsBadRequest()
        {
            // Arrange
            var dto = new ResetPasswordDTO
            {
                Token = "invalid_token",
                NewPassword = "NewPassword123!"
            };

            var user = new User
            {
                Id = 1,
                ResetTokenHash = BCrypt.Net.BCrypt.HashPassword("valid_token"),
                ResetTokenExpiry = DateTime.UtcNow.AddHours(1)
            };

            _mockResetPasswordValidator.Setup(v => v.ValidateAsync(dto, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _mockContext.Setup(c => c.Users).Returns(CreateMockDbSet(new List<User> { user }.AsQueryable()).Object);

            // Act
            var result = await _controller.ResetPassword(dto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task ResetPassword_ExpiredToken_ReturnsBadRequest()
        {
            // Arrange
            var dto = new ResetPasswordDTO
            {
                Token = "valid_token",
                NewPassword = "NewPassword123!"
            };

            var user = new User
            {
                Id = 1,
                ResetTokenHash = BCrypt.Net.BCrypt.HashPassword("valid_token"),
                ResetTokenExpiry = DateTime.UtcNow.AddHours(-1) // Истекший токен
            };

            _mockResetPasswordValidator.Setup(v => v.ValidateAsync(dto, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _mockContext.Setup(c => c.Users).Returns(CreateMockDbSet(new List<User> { user }.AsQueryable()).Object);

            // Act
            var result = await _controller.ResetPassword(dto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        #endregion

        #region ChangePassword Tests

        [Fact]
        public async Task ChangePassword_ValidData_ReturnsOkResult()
        {
            // Arrange
            var dto = new ChangePasswordDTO
            {
                CurrentPassword = "Password123!",
                NewPassword = "NewPassword123!"
            };

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            var user = new User
            {
                Id = 1,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!")
            };

            _mockChangePasswordValidator.Setup(v => v.ValidateAsync(dto, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _mockContext.Setup(c => c.Users).Returns(CreateMockDbSet(new List<User> { user }.AsQueryable()).Object);

            // Act
            var result = await _controller.ChangePassword(dto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task ChangePassword_InvalidCurrentPassword_ReturnsBadRequest()
        {
            // Arrange
            var dto = new ChangePasswordDTO
            {
                CurrentPassword = "WrongPassword",
                NewPassword = "NewPassword123!"
            };

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            var user = new User
            {
                Id = 1,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!")
            };

            _mockChangePasswordValidator.Setup(v => v.ValidateAsync(dto, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _mockContext.Setup(c => c.Users).Returns(CreateMockDbSet(new List<User> { user }.AsQueryable()).Object);

            // Act
            var result = await _controller.ChangePassword(dto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        #endregion

        #region ConfirmEmail Tests

        [Fact]
        public async Task ConfirmEmail_ValidToken_ReturnsOkResult()
        {
            // Arrange
            var dto = new ConfirmEmailDTO
            {
                UserId = 1,
                Token = "valid_token"
            };

            var user = new User
            {
                Id = 1,
                IsEmailConfirmed = false
            };

            _mockConfirmEmailValidator.Setup(v => v.ValidateAsync(dto, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _mockContext.Setup(c => c.Users).Returns(CreateMockDbSet(new List<User> { user }.AsQueryable()).Object);

            // Act
            var result = await _controller.ConfirmEmail(dto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task ConfirmEmail_InvalidToken_ReturnsBadRequest()
        {
            // Arrange
            var dto = new ConfirmEmailDTO
            {
                UserId = 1,
                Token = ""
            };

            var user = new User
            {
                Id = 1,
                IsEmailConfirmed = false
            };

            _mockConfirmEmailValidator.Setup(v => v.ValidateAsync(dto, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _mockContext.Setup(c => c.Users).Returns(CreateMockDbSet(new List<User> { user }.AsQueryable()).Object);

            // Act
            var result = await _controller.ConfirmEmail(dto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        #endregion

        #region Profile Tests

        [Fact]
        public async Task Profile_AuthenticatedUser_ReturnsOkResult()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            var user = new User
            {
                Id = 1,
                Email = "test@example.com",
                Username = "testuser",
                IsActive = true,
                IsDeleted = false,
                UserRoles = new List<UserRole>()
            };

            _mockContext.Setup(c => c.Users).Returns(CreateMockDbSet(new List<User> { user }.AsQueryable()).Object);

            // Act
            var result = await _controller.Profile();

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
        }

        #endregion

        #region Helper Methods

        private Mock<DbSet<T>> CreateMockDbSet<T>(IQueryable<T> data) where T : class
        {
            return AsyncTestHelpers.CreateMockDbSet(data);
        }

        #endregion
    }
}