using OnlineStore.Core.DTOs;
using Xunit;

namespace OnlineStore.Tests.DTOs
{
    public class CartItemDtosTests
    {
        [Fact]
        public void CartItemCreateDto_Should_Create_Instance_With_Correct_Properties()
        {
            // Arrange
            var productId = 1;
            var quantity = 2;

            // Act
            var dto = new CartItemCreateDto
            {
                ProductId = productId,
                Quantity = quantity
            };

            // Assert
            Assert.Equal(productId, dto.ProductId);
            Assert.Equal(quantity, dto.Quantity);
        }

        [Fact]
        public void CartItemUpdateDto_Should_Create_Instance_With_Correct_Properties()
        {
            // Arrange
            var quantity = 3;

            // Act
            var dto = new CartItemUpdateDto
            {
                Quantity = quantity
            };

            // Assert
            Assert.Equal(quantity, dto.Quantity);
        }

        [Fact]
        public void CartItemResponseDto_Should_Create_Instance_With_Correct_Properties()
        {
            // Arrange
            var id = 1;
            var productId = 2;
            var productName = "Test Product";
            var productPrice = 100m;
            var quantity = 3;
            var totalPrice = 300m;

            // Act
            var dto = new CartItemResponseDto
            {
                Id = id,
                ProductId = productId,
                ProductName = productName,
                ProductPrice = productPrice,
                Quantity = quantity,
                TotalPrice = totalPrice
            };

            // Assert
            Assert.Equal(id, dto.Id);
            Assert.Equal(productId, dto.ProductId);
            Assert.Equal(productName, dto.ProductName);
            Assert.Equal(productPrice, dto.ProductPrice);
            Assert.Equal(quantity, dto.Quantity);
            Assert.Equal(totalPrice, dto.TotalPrice);
        }
    }
}