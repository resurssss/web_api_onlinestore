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
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace OnlineStore.Tests.Controllers
{
    public class AuthControllerTests
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

        public AuthControllerTests()
        {
            _mockContext = new Mock<OnlineStoreDbContext>(new DbContextOptions<OnlineStoreDbContext>());
            _mockRegisterValidator = new Mock<IValidator<RegisterDTO>>();
            _mockForgotPasswordValidator = new Mock<IValidator<ForgotPasswordDTO>>();
            _mockResetPasswordValidator = new Mock<IValidator<ResetPasswordDTO>>();
            _mockChangePasswordValidator = new Mock<IValidator<ChangePasswordDTO>>();
            _mockConfirmEmailValidator = new Mock<IValidator<ConfirmEmailDTO>>();
            _mockJwtService = new Mock<IJwtService>();
            _mockAuditService = new Mock<IAuditService>();

            // Настройка пустых DbSet по умолчанию
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
            // Настройка пустых DbSet
            _mockContext.Setup(c => c.Users).Returns(CreateMockDbSet(new List<User>().AsQueryable()).Object);
            _mockContext.Setup(c => c.Roles).Returns(CreateMockDbSet(new List<Role>().AsQueryable()).Object);
            _mockContext.Setup(c => c.UserRoles).Returns(CreateMockDbSet(new List<UserRole>().AsQueryable()).Object);
            _mockContext.Setup(c => c.UserSessions).Returns(CreateMockDbSet(new List<UserSession>().AsQueryable()).Object);
            // Удалены несуществующие DbSet
        }

        #region Register Tests

        [Fact]
        public async Task Register_ValidData_ReturnsOkResult()
        {
            // Arrange
            var dto = new RegisterDTO
            {
                Email = "test@example.com",
                Username = "testuser",
                Password = "Password123!",
                FirstName = "Test",
                LastName = "User",
                DateOfBirth = new DateTime(1990, 1, 1),
                PhoneNumber = "+1234567890"
            };

            var roles = new List<Role> { new Role { Id = 1, Name = "Пользователь" } };
            var mockRolesDbSet = CreateMockDbSet(roles.AsQueryable());

            _mockContext.Setup(c => c.Roles).Returns(mockRolesDbSet.Object);
            
            // Настройка Users DbSet для проверки существующих пользователей
            var users = new List<User>();
            var mockUsersDbSet = CreateMockDbSet(users.AsQueryable());
            _mockContext.Setup(c => c.Users).Returns(mockUsersDbSet.Object);

            // Настройка UserRoles DbSet
            var userRoles = new List<UserRole>();
            var mockUserRolesDbSet = CreateMockDbSet(userRoles.AsQueryable());
            _mockContext.Setup(c => c.UserRoles).Returns(mockUserRolesDbSet.Object);

            _mockRegisterValidator.Setup(v => v.ValidateAsync(dto, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            // Настройка SaveChangesAsync
            _mockContext.Setup(c => c.SaveChangesAsync(default))
                .ReturnsAsync(1);

            // Act
            var result = await _controller.Register(dto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Register_InvalidEmail_ReturnsBadRequest()
        {
            // Arrange
            var dto = new RegisterDTO
            {
                Email = "invalid-email",
                Username = "testuser",
                Password = "Password123!",
                FirstName = "Test",
                LastName = "User",
                DateOfBirth = new DateTime(1990, 1, 1),
                PhoneNumber = "+1234567890"
            };

            var validationResult = new FluentValidation.Results.ValidationResult();
            validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("Email", "Некорректный email"));

            _mockRegisterValidator.Setup(v => v.ValidateAsync(dto, default))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _controller.Register(dto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Register_ExistingEmail_ReturnsBadRequest()
        {
            // Arrange
            var dto = new RegisterDTO
            {
                Email = "existing@example.com",
                Username = "testuser",
                Password = "Password123!",
                FirstName = "Test",
                LastName = "User",
                DateOfBirth = new DateTime(1990, 1, 1),
                PhoneNumber = "+1234567890"
            };

            var existingUsers = new List<User>
            {
                new User { Email = "existing@example.com", Username = "differentuser" }
            };

            var mockUsersDbSet = CreateMockDbSet(existingUsers.AsQueryable());
            _mockContext.Setup(c => c.Users).Returns(mockUsersDbSet.Object);

            _mockRegisterValidator.Setup(v => v.ValidateAsync(dto, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            // Act
            var result = await _controller.Register(dto);

            // Assert
            // Проверка, что возвращается BadRequest при существующем email
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Register_ExistingUsername_ReturnsBadRequest()
        {
            // Arrange
            var dto = new RegisterDTO
            {
                Email = "test@example.com",
                Username = "existinguser",
                Password = "Password123!",
                FirstName = "Test",
                LastName = "User",
                DateOfBirth = new DateTime(1990, 1, 1),
                PhoneNumber = "+1234567890"
            };

            var existingUsers = new List<User>
            {
                new User { Email = "different@example.com", Username = "existinguser" }
            };

            var mockUsersDbSet = CreateMockDbSet(existingUsers.AsQueryable());
            _mockContext.Setup(c => c.Users).Returns(mockUsersDbSet.Object);

            _mockRegisterValidator.Setup(v => v.ValidateAsync(dto, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            // Act
            var result = await _controller.Register(dto);

            // Assert
            // Проверка, что возвращается BadRequest при существующем username
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Register_PasswordMismatch_ReturnsBadRequest()
        {
            // Arrange
            var dto = new RegisterDTO
            {
                Email = "test@example.com",
                Username = "testuser",
                Password = "Password123!",
                FirstName = "Test",
                LastName = "User",
                DateOfBirth = new DateTime(1990, 1, 1),
                PhoneNumber = "+1234567890"
            };

            var validationResult = new FluentValidation.Results.ValidationResult();
            validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("Password", "Пароли не совпадают"));

            _mockRegisterValidator.Setup(v => v.ValidateAsync(dto, default))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _controller.Register(dto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        #endregion

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

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("Password123!");
            var user = new User
            {
                Id = 1,
                Email = "test@example.com",
                Username = "testuser",
                PasswordHash = hashedPassword,
                IsActive = true,
                IsEmailConfirmed = true,
                IsDeleted = false,
                UserRoles = new List<UserRole> { new UserRole { UserId = 1, RoleId = 1, Role = new Role { Id = 1, Name = "Пользователь" } } }
            };

            var users = new List<User> { user };
            var mockUsersDbSet = CreateMockDbSet(users.AsQueryable());
            _mockContext.Setup(c => c.Users).Returns(mockUsersDbSet.Object);
            
            _mockJwtService.Setup(j => j.GenerateAccessToken(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()))
                .Returns("access_token");
            _mockJwtService.Setup(j => j.GenerateRefreshToken())
                .Returns("refresh_token");
            
            _mockJwtService.Setup(j => j.GenerateRefreshToken())
                .Returns("refresh_token");

            _mockJwtService.Setup(j => j.GenerateAccessToken(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()))
                .Returns("access_token");
            _mockJwtService.Setup(j => j.GenerateRefreshToken())
                .Returns("refresh_token");

            _mockContext.Setup(c => c.SaveChangesAsync(default))
                .ReturnsAsync(1);

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

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("Password123!");
            var user = new User
            {
                Id = 1,
                Email = "test@example.com",
                Username = "testuser",
                PasswordHash = hashedPassword,
                IsActive = true,
                IsEmailConfirmed = true,
                IsDeleted = false,
                UserRoles = new List<UserRole> { new UserRole { UserId = 1, RoleId = 1, Role = new Role { Id = 1, Name = "Пользователь" } } }
            };

            var users = new List<User> { user };
            var mockUsersDbSet = CreateMockDbSet(users.AsQueryable());
            _mockContext.Setup(c => c.Users).Returns(mockUsersDbSet.Object);

            _mockJwtService.Setup(j => j.GenerateAccessToken(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()))
                .Returns("access_token");
            _mockJwtService.Setup(j => j.GenerateRefreshToken())
                .Returns("refresh_token");

            _mockContext.Setup(c => c.SaveChangesAsync(default))
                .ReturnsAsync(1);

            // Act
            var result = await _controller.Login(dto);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
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

            var users = new List<User>();
            var mockUsersDbSet = CreateMockDbSet(users.AsQueryable());
            _mockContext.Setup(c => c.Users).Returns(mockUsersDbSet.Object);

            // Act
            var result = await _controller.Login(dto);

            // Assert
            result.Result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task Login_InvalidPassword_ReturnsUnauthorized()
        {
            // Arrange
            var dto = new LoginDTO
            {
                EmailOrUsername = "test@example.com",
                Password = "WrongPassword!"
            };

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("Password123!");
            var user = new User
            {
                Id = 1,
                Email = "test@example.com",
                Username = "testuser",
                PasswordHash = hashedPassword,
                IsActive = true,
                IsEmailConfirmed = true,
                IsDeleted = false,
                UserRoles = new List<UserRole>()
            };

            var users = new List<User> { user };
            var mockUsersDbSet = CreateMockDbSet(users.AsQueryable());
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

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("Password123!");
            var user = new User
            {
                Id = 1,
                Email = "test@example.com",
                Username = "testuser",
                PasswordHash = hashedPassword,
                IsActive = false,
                IsEmailConfirmed = true,
                IsDeleted = false,
                UserRoles = new List<UserRole>()
            };

            var users = new List<User> { user };
            var mockUsersDbSet = CreateMockDbSet(users.AsQueryable());
            _mockContext.Setup(c => c.Users).Returns(mockUsersDbSet.Object);

            // Act
            var result = await _controller.Login(dto);

            // Assert
            result.Result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task Login_DeletedUser_ReturnsUnauthorized()
        {
            // Arrange
            var dto = new LoginDTO
            {
                EmailOrUsername = "test@example.com",
                Password = "Password123!"
            };

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("Password123!");
            var user = new User
            {
                Id = 1,
                Email = "test@example.com",
                Username = "testuser",
                PasswordHash = hashedPassword,
                IsActive = true,
                IsEmailConfirmed = true,
                IsDeleted = true,
                UserRoles = new List<UserRole>()
            };

            var users = new List<User> { user };
            var mockUsersDbSet = CreateMockDbSet(users.AsQueryable());
            _mockContext.Setup(c => c.Users).Returns(mockUsersDbSet.Object);

            // Act
            var result = await _controller.Login(dto);

            // Assert
            result.Result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task Login_EmailNotConfirmed_ReturnsBadRequest()
        {
            // Arrange
            var dto = new LoginDTO
            {
                EmailOrUsername = "test@example.com",
                Password = "Password123!"
            };

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("Password123!");
            var user = new User
            {
                Id = 1,
                Email = "test@example.com",
                Username = "testuser",
                PasswordHash = hashedPassword,
                IsActive = true,
                IsEmailConfirmed = false,
                IsDeleted = false,
                UserRoles = new List<UserRole>()
            };

            var users = new List<User> { user };
            var mockUsersDbSet = CreateMockDbSet(users.AsQueryable());
            _mockContext.Setup(c => c.Users).Returns(mockUsersDbSet.Object);
            
            _mockJwtService.Setup(j => j.GenerateAccessToken(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()))
                .Returns("access_token");
            _mockJwtService.Setup(j => j.GenerateRefreshToken())
                .Returns("refresh_token");

            // Act
            var result = await _controller.Login(dto);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
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