using OnlineStore.Core.DTOs;
using Xunit;

namespace OnlineStore.Tests.DTOs
{
    public class FavoriteDtosTests
    {
        [Fact]
        public void FavoriteCreateDto_Should_Create_Instance_With_Correct_Properties()
        {
            // Arrange
            var userId = 1;
            var productId = 2;

            // Act
            var dto = new FavoriteCreateDto
            {
                UserId = userId,
                ProductId = productId
            };

            // Assert
            Assert.Equal(userId, dto.UserId);
            Assert.Equal(productId, dto.ProductId);
        }

        [Fact]
        public void FavoriteResponseDto_Should_Create_Instance_With_Correct_Properties()
        {
            // Arrange
            var id = 1;
            var userId = 1;
            var productId = 2;
            var product = new ProductListItemDto
            {
                Id = productId,
                Name = "Test Product",
                Price = 100m,
                IsInStock = true
            };
            var createdAt = DateTime.UtcNow;
            var updatedAt = DateTime.UtcNow;

            // Act
            var dto = new FavoriteResponseDto
            {
                Id = id,
                UserId = userId,
                ProductId = productId,
                Product = product,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt
            };

            // Assert
            Assert.Equal(id, dto.Id);
            Assert.Equal(userId, dto.UserId);
            Assert.Equal(productId, dto.ProductId);
            Assert.Equal(product, dto.Product);
            Assert.Equal(createdAt, dto.CreatedAt);
            Assert.Equal(updatedAt, dto.UpdatedAt);
        }

        [Fact]
        public void FavoriteListItemDto_Should_Create_Instance_With_Correct_Properties()
        {
            // Arrange
            var id = 1;
            var productId = 2;
            var productName = "Test Product";
            var productPrice = 100m;

            // Act
            var dto = new FavoriteListItemDto
            {
                Id = id,
                ProductId = productId,
                ProductName = productName,
                ProductPrice = productPrice
            };

            // Assert
            Assert.Equal(id, dto.Id);
            Assert.Equal(productId, dto.ProductId);
            Assert.Equal(productName, dto.ProductName);
            Assert.Equal(productPrice, dto.ProductPrice);
        }
    }
}