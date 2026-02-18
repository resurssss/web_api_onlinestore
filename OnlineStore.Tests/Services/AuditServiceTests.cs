using Microsoft.EntityFrameworkCore;
using Moq;
using OnlineStore.Core;
using OnlineStore.Core.Interfaces;
using OnlineStore.Core.Models;
using OnlineStore.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using OnlineStore.Tests;
using System.Threading;

namespace OnlineStore.Tests.Services
{
    public class AuditServiceTests
    {
        private readonly Mock<OnlineStoreDbContext> _mockContext;
        private readonly AuditService _auditService;

        public AuditServiceTests()
        {
            var options = new DbContextOptions<OnlineStoreDbContext>();
            _mockContext = new Mock<OnlineStoreDbContext>(options);
            _auditService = new AuditService(_mockContext.Object);
            
            // Setup default empty DbSets
            _mockContext.Setup(c => c.SecurityAuditLogs).Returns(CreateMockDbSet<SecurityAuditLog>(new List<SecurityAuditLog>().AsQueryable()).Object);
        }

        #region Security Event Logging Tests

        [Fact]
        public async Task LogEventAsync_ShouldAddAuditLog()
        {
            // Arrange
            var auditLogs = new List<SecurityAuditLog>().AsQueryable();
            var mockDbSet = CreateMockDbSet(auditLogs);
            _mockContext.Setup(c => c.SecurityAuditLogs).Returns(mockDbSet.Object);

            // Act
            await _auditService.LogEventAsync(
                SecurityEventType.Login,
                1,
                "test@example.com",
                "127.0.0.1",
                "Mozilla/5.0",
                true,
                new { Message = "User logged in" }
            );

            // Assert
            mockDbSet.Verify(m => m.Add(It.IsAny<SecurityAuditLog>()), Times.Once());
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once());
        }

        #endregion

        #region Get Audit Logs Tests


        [Fact]
        public async Task GetUserAuditLogsAsync_ShouldReturnUserLogs()
        {
            // Arrange
            var userId = 1;
            var auditLogs = new List<SecurityAuditLog>
            {
                new SecurityAuditLog
                {
                    Id = Guid.NewGuid(),
                    EventType = SecurityEventType.Login,
                    UserId = userId,
                    Email = "test@example.com",
                    IpAddress = "127.0.0.1",
                    UserAgent = "Mozilla/5.0",
                    Success = true,
                    Timestamp = DateTime.UtcNow
                },
                new SecurityAuditLog
                {
                    Id = Guid.NewGuid(),
                    EventType = SecurityEventType.Login,
                    UserId = 2, // Другой пользователь
                    Email = "other@example.com",
                    IpAddress = "127.0.0.1",
                    UserAgent = "Mozilla/5.0",
                    Success = true,
                    Timestamp = DateTime.UtcNow
                }
            }.AsQueryable();

            var mockDbSet = CreateMockDbSet(auditLogs);
            _mockContext.Setup(c => c.SecurityAuditLogs).Returns(mockDbSet.Object);

            // Act
            var result = await _auditService.GetUserAuditLogsAsync(userId, 1, 10);

            // Assert
            result.Should().HaveCount(1);
            result.First().UserId.Should().Be(userId);
        }

        #endregion

        #region Suspicious Activity Tests

        [Fact]
        public async Task GetSuspiciousActivityAsync_ShouldReturnSuspiciousLogs()
        {
            // Arrange
            var auditLogs = new List<SecurityAuditLog>
            {
                new SecurityAuditLog
                {
                    Id = Guid.NewGuid(),
                    EventType = SecurityEventType.FailedLogin,
                    UserId = 1,
                    Email = "test@example.com",
                    IpAddress = "127.0.0.1",
                    UserAgent = "Mozilla/5.0",
                    Success = false,
                    Timestamp = DateTime.UtcNow
                },
                new SecurityAuditLog
                {
                    Id = Guid.NewGuid(),
                    EventType = SecurityEventType.Login,
                    UserId = 2,
                    Email = "other@example.com",
                    IpAddress = "127.0.0.1",
                    UserAgent = "Mozilla/5.0",
                    Success = true,
                    Timestamp = DateTime.UtcNow
                }
            }.AsQueryable();

            var mockDbSet = CreateMockDbSet(auditLogs);
            _mockContext.Setup(c => c.SecurityAuditLogs).Returns(mockDbSet.Object);

            // Act
            var result = await _auditService.GetSuspiciousActivityAsync(1, 10);

            // Assert
            result.Should().HaveCount(1);
            result.First().EventType.Should().Be(SecurityEventType.FailedLogin);
        }

        #endregion

        #region Helper Methods

        private Mock<DbSet<T>> CreateMockDbSet<T>(IQueryable<T> data) where T : class
        {
            return AsyncTestHelpers.CreateMockDbSet(data.ToList());
        }

        #endregion
    }
}