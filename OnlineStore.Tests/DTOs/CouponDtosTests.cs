using OnlineStore.Core.DTOs;
using Xunit;

namespace OnlineStore.Tests.DTOs
{
    public class CouponDtosTests
    {
        [Fact]
        public void CouponCreateDto_Should_Create_Instance_With_Correct_Properties()
        {
            // Arrange
            var code = "TEST123";
            var discountPercent = 10m;
            var expirationDate = DateTime.UtcNow.AddDays(30);
            var usageLimit = 100;

            // Act
            var dto = new CouponCreateDto
            {
                Code = code,
                DiscountPercent = discountPercent,
                ExpirationDate = expirationDate,
                UsageLimit = usageLimit
            };

            // Assert
            Assert.Equal(code, dto.Code);
            Assert.Equal(discountPercent, dto.DiscountPercent);
            Assert.Equal(expirationDate, dto.ExpirationDate);
            Assert.Equal(usageLimit, dto.UsageLimit);
        }

        [Fact]
        public void CouponUpdateDto_Should_Create_Instance_With_Correct_Properties()
        {
            // Arrange
            var code = "UPDATED123";
            var discountPercent = 15m;
            var expirationDate = DateTime.UtcNow.AddDays(60);
            var usageLimit = 200;

            // Act
            var dto = new CouponUpdateDto
            {
                Code = code,
                DiscountPercent = discountPercent,
                ExpirationDate = expirationDate,
                UsageLimit = usageLimit
            };

            // Assert
            Assert.Equal(code, dto.Code);
            Assert.Equal(discountPercent, dto.DiscountPercent);
            Assert.Equal(expirationDate, dto.ExpirationDate);
            Assert.Equal(usageLimit, dto.UsageLimit);
        }

        [Fact]
        public void CouponResponseDto_Should_Create_Instance_With_Correct_Properties()
        {
            // Arrange
            var id = 1;
            var code = "TEST123";
            var discountPercent = 10m;
            var expirationDate = DateTime.UtcNow.AddDays(30);
            var usageLimit = 100;
            var timesUsed = 5;
            var isActive = true;
            var createdAt = DateTime.UtcNow;
            var updatedAt = DateTime.UtcNow;

            // Act
            var dto = new CouponResponseDto
            {
                Id = id,
                Code = code,
                DiscountPercent = discountPercent,
                ExpirationDate = expirationDate,
                UsageLimit = usageLimit,
                TimesUsed = timesUsed,
                IsActive = isActive,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt
            };

            // Assert
            Assert.Equal(id, dto.Id);
            Assert.Equal(code, dto.Code);
            Assert.Equal(discountPercent, dto.DiscountPercent);
            Assert.Equal(expirationDate, dto.ExpirationDate);
            Assert.Equal(usageLimit, dto.UsageLimit);
            Assert.Equal(timesUsed, dto.TimesUsed);
            Assert.Equal(isActive, dto.IsActive);
            Assert.Equal(createdAt, dto.CreatedAt);
            Assert.Equal(updatedAt, dto.UpdatedAt);
        }

        [Fact]
        public void CouponListItemDto_Should_Create_Instance_With_Correct_Properties()
        {
            // Arrange
            var id = 1;
            var code = "TEST123";
            var discountPercent = 10m;
            var isActive = true;

            // Act
            var dto = new CouponListItemDto
            {
                Id = id,
                Code = code,
                DiscountPercent = discountPercent,
                IsActive = isActive
            };

            // Assert
            Assert.Equal(id, dto.Id);
            Assert.Equal(code, dto.Code);
            Assert.Equal(discountPercent, dto.DiscountPercent);
            Assert.Equal(isActive, dto.IsActive);
        }
    }
}