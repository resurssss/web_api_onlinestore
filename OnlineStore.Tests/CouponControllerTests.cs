using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OnlineStore.Core.DTOs;
using OnlineStore.Services.Services;
using Xunit;
using OnlineStore.API.Controllers;
using OnlineStore.Core.Interfaces;

namespace OnlineStore.Tests
{
    public class CouponControllerTests
    {
        private readonly Mock<ICouponService> _mockCouponService;
        private readonly CouponsController _couponController;

        public CouponControllerTests()
        {
            _mockCouponService = new Mock<ICouponService>();
            _couponController = new CouponsController(_mockCouponService.Object);
        }

        [Fact]
        public async Task GetAll_WhenCouponsExist_ReturnsOkResultWithCoupons()
        {
            // Arrange
            var coupons = new List<CouponListItemDto>
            {
                new CouponListItemDto { Id = 1, Code = "TEST123", DiscountPercent = 10 },
                new CouponListItemDto { Id = 2, Code = "TEST456", DiscountPercent = 15 }
            };

            _mockCouponService.Setup(service => service.GetAllAsync(It.IsAny<string>(), CancellationToken.None))
                .ReturnsAsync(coupons);

            // Act
            var result = await _couponController.GetAll("TEST", CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<CouponListItemDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetById_WhenCouponExists_ReturnsOkResultWithCoupon()
        {
            // Arrange
            var coupon = new CouponResponseDto { Id = 1, Code = "TEST123", DiscountPercent = 10 };
            
            _mockCouponService.Setup(service => service.GetByIdAsync(1, CancellationToken.None))
                .ReturnsAsync(coupon);

            // Act
            var result = await _couponController.GetById(1, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<CouponResponseDto>(okResult.Value);
            Assert.Equal(1, returnValue.Id);
            Assert.Equal("TEST123", returnValue.Code);
        }

        [Fact]
        public async Task GetById_WhenCouponDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            _mockCouponService.Setup(service => service.GetByIdAsync(999, CancellationToken.None))
                .ReturnsAsync((CouponResponseDto?)null);

            // Act
            var result = await _couponController.GetById(999, CancellationToken.None);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Create_WhenCouponIsValid_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var couponCreateDto = new CouponCreateDto { Code = "NEW123", DiscountPercent = 10 };
            var couponResponseDto = new CouponResponseDto { Id = 1, Code = "NEW123", DiscountPercent = 10 };

            _mockCouponService.Setup(service => service.CreateAsync(couponCreateDto, CancellationToken.None))
                .ReturnsAsync(couponResponseDto);

            // Act
            var result = await _couponController.Create(couponCreateDto, CancellationToken.None);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<CouponResponseDto>(createdResult.Value);
            Assert.Equal(1, returnValue.Id);
            Assert.Equal("NEW123", returnValue.Code);
        }

        [Fact]
        public async Task Create_WhenCouponIsInvalid_ReturnsBadRequest()
        {
            // Arrange
            var couponCreateDto = new CouponCreateDto { Code = "INVALID", DiscountPercent = 10 };

            _mockCouponService.Setup(service => service.CreateAsync(couponCreateDto, CancellationToken.None))
                .ReturnsAsync((CouponResponseDto?)null);

            // Act
            var result = await _couponController.Create(couponCreateDto, CancellationToken.None);

            // Assert
            Assert.IsType<BadRequestResult>(result.Result);
        }

        [Fact]
        public async Task Update_WhenCouponIsValid_ReturnsOkResultWithCoupon()
        {
            // Arrange
            var couponUpdateDto = new CouponUpdateDto { Code = "UPDATED123", DiscountPercent = 15 };
            var couponResponseDto = new CouponResponseDto { Id = 1, Code = "UPDATED123", DiscountPercent = 15 };

            _mockCouponService.Setup(service => service.UpdateAsync(1, couponUpdateDto, CancellationToken.None))
                .ReturnsAsync(couponResponseDto);

            // Act
            var result = await _couponController.Update("1", couponUpdateDto, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<CouponResponseDto>(okResult.Value);
            Assert.Equal(1, returnValue.Id);
            Assert.Equal("UPDATED123", returnValue.Code);
        }

        [Fact]
        public async Task Update_WhenCouponDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var couponUpdateDto = new CouponUpdateDto { Code = "NONEXISTENT", DiscountPercent = 15 };

            _mockCouponService.Setup(service => service.UpdateAsync(999, couponUpdateDto, CancellationToken.None))
                .ReturnsAsync((CouponResponseDto?)null);

            // Act
            var result = await _couponController.Update("999", couponUpdateDto, CancellationToken.None);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Update_WhenInvalidIdFormat_ReturnsBadRequest()
        {
            // Arrange
            var couponUpdateDto = new CouponUpdateDto { Code = "INVALID", DiscountPercent = 15 };

            // Act
            var result = await _couponController.Update("invalid", couponUpdateDto, CancellationToken.None);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task Delete_WhenCouponExists_ReturnsNoContent()
        {
            // Arrange
            _mockCouponService.Setup(service => service.DeleteAsync(1, CancellationToken.None))
                .ReturnsAsync(true);

            // Act
            var result = await _couponController.Delete("1", CancellationToken.None);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_WhenCouponDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            _mockCouponService.Setup(service => service.DeleteAsync(999, CancellationToken.None))
                .ReturnsAsync(false);

            // Act
            var result = await _couponController.Delete("999", CancellationToken.None);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_WhenInvalidIdFormat_ReturnsBadRequest()
        {
            // Act
            var result = await _couponController.Delete("invalid", CancellationToken.None);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}