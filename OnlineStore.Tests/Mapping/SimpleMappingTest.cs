using AutoMapper;
using OnlineStore.Core.DTOs;
using OnlineStore.Core.Mapping;
using OnlineStore.Core.Models;
using Xunit;

namespace OnlineStore.Tests.Mapping
{
    public class SimpleMappingTest
    {
        [Fact]
        public void TestProductUpdateMapping()
        {
            // Arrange
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductMappingProfile>();
            });
            var mapper = config.CreateMapper();

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
                Price = 150m
            };

            // Act
            mapper.Map(dto, product);

            // Assert
            Assert.Equal("Updated Name", product.Name);
            Assert.Equal(150m, product.Price);
            Assert.Equal(5, product.Stock); // Should not change
        }
    }
}