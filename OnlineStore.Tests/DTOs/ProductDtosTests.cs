using OnlineStore.Core.DTOs;
using Xunit;

namespace OnlineStore.Tests.DTOs
{
    public class ProductDtosTests
    {
        [Fact]
        public void ProductCreateDto_Should_Create_Instance_With_Correct_Properties()
        {
            // Arrange
            var name = "Test Product";
            var description = "Test Description";
            var price = 100m;
            var stock = 10;

            // Act
            var dto = new ProductCreateDto
            {
                Name = name,
                Description = description,
                Price = price,
                Stock = stock
            };

            // Assert
            Assert.Equal(name, dto.Name);
            Assert.Equal(description, dto.Description);
            Assert.Equal(price, dto.Price);
            Assert.Equal(stock, dto.Stock);
        }

        [Fact]
        public void ProductUpdateDto_Should_Create_Instance_With_Correct_Properties()
        {
            // Arrange
            var name = "Updated Product";
            var description = "Updated Description";
            var price = 150m;
            var stock = 20;

            // Act
            var dto = new ProductUpdateDto
            {
                Name = name,
                Description = description,
                Price = price,
                Stock = stock
            };

            // Assert
            Assert.Equal(name, dto.Name);
            Assert.Equal(description, dto.Description);
            Assert.Equal(price, dto.Price);
            Assert.Equal(stock, dto.Stock);
        }

        [Fact]
        public void ProductResponseDto_Should_Create_Instance_With_Correct_Properties()
        {
            // Arrange
            var id = 1;
            var name = "Test Product";
            var description = "Test Description";
            var price = 100m;
            var stock = 10;
            var isInStock = true;
            var createdAt = DateTime.UtcNow;
            var updatedAt = DateTime.UtcNow;
            var reviews = new List<ReviewResponseDto>
            {
                new ReviewResponseDto
                {
                    Id = 1,
                    ProductId = id,
                    Author = "Test Author",
                    Rating = 5,
                    Comment = "Test Comment",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };
            var averageRating = 5.0;

            // Act
            var dto = new ProductResponseDto
            {
                Id = id,
                Name = name,
                Description = description,
                Price = price,
                Stock = stock,
                IsInStock = isInStock,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt,
                Reviews = reviews,
                AverageRating = averageRating
            };

            // Assert
            Assert.Equal(id, dto.Id);
            Assert.Equal(name, dto.Name);
            Assert.Equal(description, dto.Description);
            Assert.Equal(price, dto.Price);
            Assert.Equal(stock, dto.Stock);
            Assert.Equal(isInStock, dto.IsInStock);
            Assert.Equal(createdAt, dto.CreatedAt);
            Assert.Equal(updatedAt, dto.UpdatedAt);
            Assert.Equal(reviews, dto.Reviews);
            Assert.Equal(averageRating, dto.AverageRating);
        }

        [Fact]
        public void ProductListItemDto_Should_Create_Instance_With_Correct_Properties()
        {
            // Arrange
            var id = 1;
            var name = "Test Product";
            var price = 100m;
            var isInStock = true;

            // Act
            var dto = new ProductListItemDto
            {
                Id = id,
                Name = name,
                Price = price,
                IsInStock = isInStock
            };

            // Assert
            Assert.Equal(id, dto.Id);
            Assert.Equal(name, dto.Name);
            Assert.Equal(price, dto.Price);
            Assert.Equal(isInStock, dto.IsInStock);
        }
    }
}