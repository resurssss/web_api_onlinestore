using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using OnlineStore.Core;
using OnlineStore.Core.DTOs;
using OnlineStore.Core.Models;
using OnlineStore.Services.Services;
using Xunit;

namespace OnlineStore.Tests.Services
{
    public class CouponServiceTests
    {
        private readonly Mock<ILogger<CouponService>> _mockLogger;
        private readonly IMapper _mapper;
        private readonly DbContextOptions<OnlineStoreDbContext> _options;

        public CouponServiceTests()
        {
            _mockLogger = new Mock<ILogger<CouponService>>();
            
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<Coupon, CouponListItemDto>();
                cfg.CreateMap<Coupon, CouponResponseDto>();
                cfg.CreateMap<CouponCreateDto, Coupon>();
                cfg.CreateMap<CouponUpdateDto, Coupon>();
            });
            _mapper = config.CreateMapper();
            
            _options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "CouponServiceTestDb")
                .Options;
        }

        [Fact]
        public async Task GetAllAsync_WhenCouponsExist_ReturnsAllCoupons()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "GetAllAsync_WhenCouponsExist_ReturnsAllCoupons_1")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var couponService = new CouponService(context, _mapper, _mockLogger.Object);
            
            var coupons = new List<Coupon>()
            {
                new Coupon { Id = 10, Code = "TEST10", DiscountPercent = 10, IsActive = true, ExpirationDate = DateTime.UtcNow.AddDays(30) },
                new Coupon { Id = 11, Code = "TEST20", DiscountPercent = 20, IsActive = true, ExpirationDate = DateTime.UtcNow.AddDays(30) }
            };
            
            context.Coupons.AddRange(coupons);
            await context.SaveChangesAsync();

            // Act
            var result = await couponService.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAllAsync_WithCodeFilter_ReturnsFilteredCoupons()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "GetAllAsync_WithCodeFilter_ReturnsFilteredCoupons_2")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var couponService = new CouponService(context, _mapper, _mockLogger.Object);
            
            var coupons = new List<Coupon>()
            {
                new Coupon { Id = 1, Code = "TEST10", DiscountPercent = 10, IsActive = true, ExpirationDate = DateTime.UtcNow.AddDays(30) },
                new Coupon { Id = 2, Code = "TEST20", DiscountPercent = 20, IsActive = true, ExpirationDate = DateTime.UtcNow.AddDays(30) }
            };
            
            context.Coupons.AddRange(coupons);
            await context.SaveChangesAsync();

            // Act
            var result = await couponService.GetAllAsync("TEST10");

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("TEST10", result.First().Code);
        }

        [Fact]
        public async Task GetCouponsAsync_ReturnsPagedResult()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "GetCouponsAsync_ReturnsPagedResult_3")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var couponService = new CouponService(context, _mapper, _mockLogger.Object);
            
            var coupons = new List<Coupon>();
            for (int i = 1; i <= 15; i++)
            {
                coupons.Add(new Coupon { Id = i, Code = $"TEST{i}", DiscountPercent = i, IsActive = true, ExpirationDate = DateTime.UtcNow.AddDays(30) });
            }
            
            context.Coupons.AddRange(coupons);
            await context.SaveChangesAsync();

            // Act
            var result = await couponService.GetCouponsAsync(1, 10);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(10, result.Items.Count());
            Assert.Equal(1, result.PageNumber);
            Assert.Equal(10, result.PageSize);
            Assert.Equal(2, result.TotalPages);
            Assert.Equal(15, result.TotalCount);
        }

        [Fact]
        public async Task GetByIdAsync_WhenCouponExists_ReturnsCoupon()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "GetByIdAsync_WhenCouponExists_ReturnsCoupon_4")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var couponService = new CouponService(context, _mapper, _mockLogger.Object);
            
            var coupon = new Coupon { Id = 1, Code = "TEST10", DiscountPercent = 10, IsActive = true, ExpirationDate = DateTime.UtcNow.AddDays(30) };
            context.Coupons.Add(coupon);
            await context.SaveChangesAsync();

            // Act
            var result = await couponService.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("TEST10", result.Code);
        }

        [Fact]
        public async Task GetByIdAsync_WhenCouponDoesNotExist_ReturnsNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "GetByIdAsync_WhenCouponDoesNotExist_ReturnsNull_5")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var couponService = new CouponService(context, _mapper, _mockLogger.Object);

            // Act
            var result = await couponService.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_WhenCodeIsUnique_CreatesNewCoupon()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "CreateAsync_WhenCodeIsUnique_CreatesNewCoupon_6")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var couponService = new CouponService(context, _mapper, _mockLogger.Object);
            
            var couponDto = new CouponCreateDto
            {
                Code = "NEWCOUPON",
                DiscountPercent = 15,
                ExpirationDate = DateTime.UtcNow.AddDays(30),
                UsageLimit = 100
            };

            // Act
            var result = await couponService.CreateAsync(couponDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("NEWCOUPON", result.Code);
            Assert.Equal(1, context.Coupons.Count());
        }

        [Fact]
        public async Task CreateAsync_WhenCodeAlreadyExists_ReturnsNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "CreateAsync_WhenCodeAlreadyExists_ReturnsNull_7")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var couponService = new CouponService(context, _mapper, _mockLogger.Object);
            
            var existingCoupon = new Coupon { Id = 1, Code = "EXISTING", DiscountPercent = 10, IsActive = true, ExpirationDate = DateTime.UtcNow.AddDays(30) };
            context.Coupons.Add(existingCoupon);
            await context.SaveChangesAsync();
            
            var couponDto = new CouponCreateDto
            {
                Code = "EXISTING",
                DiscountPercent = 15,
                ExpirationDate = DateTime.UtcNow.AddDays(30),
                UsageLimit = 100
            };

            // Act
            var result = await couponService.CreateAsync(couponDto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_WhenCouponExists_UpdatesCoupon()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "UpdateAsync_WhenCouponExists_UpdatesCoupon_8")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var couponService = new CouponService(context, _mapper, _mockLogger.Object);
            
            var coupon = new Coupon { Id = 1, Code = "TEST10", DiscountPercent = 10, IsActive = true, ExpirationDate = DateTime.UtcNow.AddDays(30) };
            context.Coupons.Add(coupon);
            await context.SaveChangesAsync();
            
            var updateDto = new CouponUpdateDto
            {
                Code = "UPDATED",
                DiscountPercent = 20
            };

            // Act
            var result = await couponService.UpdateAsync(1, updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("UPDATED", result.Code);
            Assert.Equal(20, result.DiscountPercent);
            Assert.True(result.IsActive); // IsActive не изменяется при обновлении
        }

        [Fact]
        public async Task UpdateAsync_WhenCouponDoesNotExist_ReturnsNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "UpdateAsync_WhenCouponDoesNotExist_ReturnsNull_9")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var couponService = new CouponService(context, _mapper, _mockLogger.Object);
            
            var updateDto = new CouponUpdateDto
            {
                Code = "UPDATED",
                DiscountPercent = 20
            };

            // Act
            var result = await couponService.UpdateAsync(999, updateDto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_WhenCodeAlreadyExists_ReturnsNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "UpdateAsync_WhenCodeAlreadyExists_ReturnsNull_10")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var couponService = new CouponService(context, _mapper, _mockLogger.Object);
            
            var coupon1 = new Coupon { Id = 1, Code = "TEST10", DiscountPercent = 10, IsActive = true, ExpirationDate = DateTime.UtcNow.AddDays(30) };
            var coupon2 = new Coupon { Id = 2, Code = "TEST20", DiscountPercent = 20, IsActive = true, ExpirationDate = DateTime.UtcNow.AddDays(30) };
            context.Coupons.AddRange(coupon1, coupon2);
            await context.SaveChangesAsync();
            
            var updateDto = new CouponUpdateDto
            {
                Code = "TEST20", // This code already exists
                DiscountPercent = 15
            };

            // Act
            var result = await couponService.UpdateAsync(1, updateDto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteAsync_WhenCouponExists_DeletesCoupon()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "DeleteAsync_WhenCouponExists_DeletesCoupon_11")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var couponService = new CouponService(context, _mapper, _mockLogger.Object);
            
            var coupon = new Coupon { Id = 1, Code = "TEST10", DiscountPercent = 10, IsActive = true, ExpirationDate = DateTime.UtcNow.AddDays(30) };
            context.Coupons.Add(coupon);
            await context.SaveChangesAsync();

            // Act
            var result = await couponService.DeleteAsync(1);

            // Assert
            Assert.True(result);
            Assert.Equal(0, context.Coupons.Count());
        }

        [Fact]
        public async Task DeleteAsync_WhenCouponDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "DeleteAsync_WhenCouponDoesNotExist_ReturnsFalse_12")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var couponService = new CouponService(context, _mapper, _mockLogger.Object);

            // Act
            var result = await couponService.DeleteAsync(999);

            // Assert
            Assert.False(result);
        }
    }
}