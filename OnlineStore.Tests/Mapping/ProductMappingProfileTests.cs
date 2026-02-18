using AutoMapper;
using OnlineStore.Core.DTOs;
using OnlineStore.Core.Mapping;
using OnlineStore.Core.Models;
using Xunit;

namespace OnlineStore.Tests.Mapping
{
    public class ProductMappingProfileTests
    {
        private readonly IMapper _mapper;

        public ProductMappingProfileTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductMappingProfile>();
                cfg.AddProfile<ReviewProfile>(); // Добавляем ReviewProfile для маппинга отзывов
            });
            _mapper = config.CreateMapper();
        }

        [Fact]
        public void ProductCreateDto_To_Product_Mapping_Should_Work()
        {
            // Arrange
            var dto = new ProductCreateDto
            {
                Name = "Test Product",
                Description = "Test Description",
                Price = 100m,
                Stock = 10
            };

            // Act
            var product = _mapper.Map<Product>(dto);

            // Assert
            Assert.Equal(dto.Name, product.Name);
            Assert.Equal(dto.Description, product.Description);
            Assert.Equal(dto.Price, product.Price);
            Assert.Equal(dto.Stock, product.Stock);
            Assert.True(product.IsActive); // Default value
        }

        [Fact]
        public void ProductUpdateDto_To_Product_Mapping_Should_Work_With_Non_Null_Properties()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                Name = "Original Name",
                Description = "Original Description",
                Price = 50m,
                Stock = 5,
                IsActive = true,
                CategoryId = 1
            };

            var dto = new ProductUpdateDto
            {
                Name = "Updated Name",
                Description = null, // Should be ignored
                Price = 150m
                // Stock is not set, should be ignored
            };

            // Act
            _mapper.Map(dto, product);

            // Assert
            Assert.Equal(dto.Name, product.Name);
            Assert.Equal("Original Description", product.Description); // Should not change
            Assert.Equal(dto.Price, product.Price);
            Assert.Equal(5, product.Stock); // Should not change
            Assert.Equal(1, product.Id); // Should not change
            Assert.Equal(1, product.CategoryId); // Should not change
            Assert.True(product.IsActive); // Should not change
        }

        [Fact]
        public void Product_To_ProductResponseDto_Mapping_Should_Work()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                Name = "Test Product",
                Description = "Test Description",
                Price = 100m,
                Stock = 10,
                IsActive = true,
                CategoryId = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Reviews = new List<Review>
                {
                    new Review
                    {
                        Id = 1,
                        ProductId = 1,
                        Author = "Test Author",
                        Rating = 5,
                        Comment = "Great product!",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }
                }
            };

            // Act
            var dto = _mapper.Map<ProductResponseDto>(product);

            // Assert
            Assert.Equal(product.Id, dto.Id);
            Assert.Equal(product.Name, dto.Name);
            Assert.Equal(product.Description, dto.Description);
            Assert.Equal(product.Price, dto.Price);
            Assert.Equal(product.Stock, dto.Stock);
            Assert.Equal(product.Stock > 0, dto.IsInStock);
            Assert.Equal(product.CreatedAt, dto.CreatedAt);
            Assert.Equal(product.UpdatedAt, dto.UpdatedAt);
            Assert.Equal(product.Reviews.Count, dto.Reviews.Count);
            Assert.Equal(5.0, dto.AverageRating);
        }

        [Fact]
        public void Product_To_ProductListItemDto_Mapping_Should_Work()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                Name = "Test Product",
                Price = 100m,
                Stock = 10,
                IsActive = true,
                CategoryId = 1
            };

            // Act
            var dto = _mapper.Map<ProductListItemDto>(product);

            // Assert
            Assert.Equal(product.Id, dto.Id);
            Assert.Equal(product.Name, dto.Name);
            Assert.Equal(product.Price, dto.Price);
            Assert.Equal(product.Stock > 0, dto.IsInStock);
        }
    }
}