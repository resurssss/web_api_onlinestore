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
    public class ProductServiceTests
    {
        private readonly Mock<ILogger<ProductService>> _mockLogger;
        private readonly IMapper _mapper;
        private readonly DbContextOptions<OnlineStoreDbContext> _options;

        public ProductServiceTests()
        {
            _mockLogger = new Mock<ILogger<ProductService>>();
            
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<Product, ProductResponseDto>();
                cfg.CreateMap<Product, ProductListItemDto>();
                cfg.CreateMap<ProductCreateDto, Product>();
                cfg.CreateMap<ProductUpdateDto, Product>();
            });
            _mapper = config.CreateMapper();
            
            _options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "ProductServiceTestDb")
                .Options;
        }

        [Fact]
        public async Task GetProductAsync_WhenProductExists_ReturnsProduct()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "GetProductAsync_WhenProductExists_ReturnsProduct_" + Guid.NewGuid())
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var productService = new ProductService(context, _mapper, _mockLogger.Object);
            
            var product = new Product { Id = 1, Name = "Test Product 1", Price = 100, Stock = 10 };
            context.Products.Add(product);
            await context.SaveChangesAsync();

            // Act
            var result = await productService.GetProductAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Product 1", result.Name);
        }

        [Fact]
        public async Task GetProductAsync_WhenProductDoesNotExist_ThrowsKeyNotFoundException()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "GetProductAsync_WhenProductDoesNotExist_ThrowsKeyNotFoundException")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var productService = new ProductService(context, _mapper, _mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => productService.GetProductAsync(999));
        }

        [Fact]
        public async Task GetProductsAsync_WithSearchFilter_ReturnsFilteredProducts()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "GetProductsAsync_WithSearchFilter_ReturnsFilteredProducts_1")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var productService = new ProductService(context, _mapper, _mockLogger.Object);
            
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Test Product 1", Price = 100, Stock = 10 },
                new Product { Id = 2, Name = "Another Product", Price = 200, Stock = 5 }
            };
            
            context.Products.AddRange(products);
            await context.SaveChangesAsync();

            // Act
            var result = await productService.GetProductsAsync(search: "Test");

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Items);
            Assert.Equal("Test Product 1", result.Items.First().Name);
        }

        [Fact]
        public async Task GetProductsAsync_WithPriceFilter_ReturnsFilteredProducts()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "GetProductsAsync_WithPriceFilter_ReturnsFilteredProducts_2")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var productService = new ProductService(context, _mapper, _mockLogger.Object);
            
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Cheap Product", Price = 50, Stock = 10 },
                new Product { Id = 2, Name = "Expensive Product", Price = 200, Stock = 5 }
            };
            
            context.Products.AddRange(products);
            await context.SaveChangesAsync();

            // Act
            var result = await productService.GetProductsAsync(minPrice: 100);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Items);
            Assert.Equal("Expensive Product", result.Items.First().Name);
        }

        [Fact]
        public async Task GetProductsAsync_WithStockFilter_ReturnsFilteredProducts()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "GetProductsAsync_WithStockFilter_ReturnsFilteredProducts_3")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var productService = new ProductService(context, _mapper, _mockLogger.Object);
            
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "In Stock Product", Price = 100, Stock = 10 },
                new Product { Id = 2, Name = "Out of Stock Product", Price = 100, Stock = 0 }
            };
            
            context.Products.AddRange(products);
            await context.SaveChangesAsync();

            // Act
            var result = await productService.GetProductsAsync(inStock: true);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Items);
            Assert.Equal("In Stock Product", result.Items.First().Name);
        }

        [Fact]
        public async Task GetProductsAsync_WithSorting_ReturnsSortedProducts()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "GetProductsAsync_WithSorting_ReturnsSortedProducts_4")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var productService = new ProductService(context, _mapper, _mockLogger.Object);
            
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "B Product", Price = 200, Stock = 10 },
                new Product { Id = 2, Name = "A Product", Price = 100, Stock = 5 }
            };
            
            context.Products.AddRange(products);
            await context.SaveChangesAsync();

            // Act
            var result = await productService.GetProductsAsync(sortBy: "name");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Items.Count());
            Assert.Equal("A Product", result.Items.First().Name);
        }

        [Fact]
        public async Task GetProductsAsync_WithPaging_ReturnsPagedResult()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "GetProductsAsync_WithPaging_ReturnsPagedResult_5")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var productService = new ProductService(context, _mapper, _mockLogger.Object);
            
            var products = new List<Product>();
            for (int i = 1; i <= 15; i++)
            {
                products.Add(new Product { Id = i, Name = $"Product {i}", Price = i * 10, Stock = 10 });
            }
            
            context.Products.AddRange(products);
            await context.SaveChangesAsync();

            // Act
            var result = await productService.GetProductsAsync(page: 1, pageSize: 10);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(10, result.Items.Count());
            Assert.Equal(1, result.PageNumber);
            Assert.Equal(10, result.PageSize);
            Assert.Equal(2, result.TotalPages);
            Assert.Equal(15, result.TotalCount);
        }

        [Fact]
        public async Task BulkCreateAsync_WhenProductsAreValid_CreatesProducts()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "BulkCreateAsync_WhenProductsAreValid_CreatesProducts_6")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var productService = new ProductService(context, _mapper, _mockLogger.Object);
            
            var productDtos = new List<ProductCreateDto>
            {
                new ProductCreateDto { Name = "Product 1", Price = 100, Stock = 10 },
                new ProductCreateDto { Name = "Product 2", Price = 200, Stock = 5 }
            };

            // Act
            var result = await productService.BulkCreateAsync(productDtos);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.True(result.All(r => r.Success));
            Assert.Equal(2, context.Products.Count());
        }

        [Fact]
        public async Task BulkCreateAsync_WhenProductNameAlreadyExists_ReturnsError()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "BulkCreateAsync_WhenProductNameAlreadyExists_ReturnsError_7")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var productService = new ProductService(context, _mapper, _mockLogger.Object);
            
            var existingProduct = new Product { Id = 1, Name = "Existing Product", Price = 100, Stock = 10 };
            context.Products.Add(existingProduct);
            await context.SaveChangesAsync();
            
            var productDtos = new List<ProductCreateDto>
            {
                new ProductCreateDto { Name = "Existing Product", Price = 200, Stock = 5 }
            };

            // Act
            var result = await productService.BulkCreateAsync(productDtos);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.False(result.First().Success);
        }

        [Fact]
        public async Task BulkUpdateAsync_WhenProductsExist_UpdatesProducts()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "BulkUpdateAsync_WhenProductsExist_UpdatesProducts_8")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var productService = new ProductService(context, _mapper, _mockLogger.Object);
            
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", Price = 100, Stock = 10 },
                new Product { Id = 2, Name = "Product 2", Price = 200, Stock = 5 }
            };
            
            context.Products.AddRange(products);
            await context.SaveChangesAsync();
            
            var updateItems = new List<(int Id, ProductUpdateDto Dto)>
            {
                (1, new ProductUpdateDto { Name = "Updated Product 1", Price = 150 }),
                (2, new ProductUpdateDto { Name = "Updated Product 2", Price = 250 })
            };

            // Act
            var result = await productService.BulkUpdateAsync(updateItems);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.True(result.All(r => r.Success));
            Assert.Equal("Updated Product 1", context.Products.First(p => p.Id == 1).Name);
            Assert.Equal(150, context.Products.First(p => p.Id == 1).Price);
        }

        [Fact]
        public async Task BulkUpdateAsync_WhenProductDoesNotExist_ReturnsError()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "BulkUpdateAsync_WhenProductDoesNotExist_ReturnsError_9")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var productService = new ProductService(context, _mapper, _mockLogger.Object);
            
            var updateItems = new List<(int Id, ProductUpdateDto Dto)>
            {
                (999, new ProductUpdateDto { Name = "Non-existent Product", Price = 150 })
            };

            // Act
            var result = await productService.BulkUpdateAsync(updateItems);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.False(result.First().Success);
        }

        [Fact]
        public async Task BulkDeleteAsync_WhenProductsExist_DeletesProducts()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "BulkDeleteAsync_WhenProductsExist_DeletesProducts_10")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var productService = new ProductService(context, _mapper, _mockLogger.Object);
            
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", Price = 100, Stock = 10 },
                new Product { Id = 2, Name = "Product 2", Price = 200, Stock = 5 }
            };
            
            context.Products.AddRange(products);
            await context.SaveChangesAsync();

            // Act
            var result = await productService.BulkDeleteAsync(new List<int> { 1, 2 });

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.True(result.All(r => r.Success));
            Assert.Equal(0, context.Products.Count());
        }

        [Fact]
        public async Task BulkDeleteAsync_WhenProductDoesNotExist_ReturnsError()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "BulkDeleteAsync_WhenProductDoesNotExist_ReturnsError_11")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var productService = new ProductService(context, _mapper, _mockLogger.Object);

            // Act
            var result = await productService.BulkDeleteAsync(new List<int> { 999 });

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.False(result.First().Success);
        }

        [Fact]
        public async Task SearchProductsAsync_WhenProductsMatch_ReturnsMatchingProducts()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "SearchProductsAsync_WhenProductsMatch_ReturnsMatchingProducts_12")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var productService = new ProductService(context, _mapper, _mockLogger.Object);
            
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Test Product", Description = "A test product", Price = 100, Stock = 10 },
                new Product { Id = 2, Name = "Another Product", Description = "Another test item", Price = 200, Stock = 5 }
            };
            
            context.Products.AddRange(products);
            await context.SaveChangesAsync();

            // Act
            var result = await productService.SearchProductsAsync("test");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetProductsByPriceRangeAsync_WhenProductsInPriceRange_ReturnsProducts()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "GetProductsByPriceRangeAsync_WhenProductsInPriceRange_ReturnsProducts_13")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var productService = new ProductService(context, _mapper, _mockLogger.Object);
            
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Cheap Product", Price = 50, Stock = 10 },
                new Product { Id = 2, Name = "Mid Price Product", Price = 150, Stock = 5 },
                new Product { Id = 3, Name = "Expensive Product", Price = 250, Stock = 3 }
            };
            
            context.Products.AddRange(products);
            await context.SaveChangesAsync();

            // Act
            var result = await productService.GetProductsByPriceRangeAsync(100, 200);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Mid Price Product", result.First().Name);
        }

        [Fact]
        public async Task GetProductStatisticsAsync_ReturnsProductStatistics()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "GetProductStatisticsAsync_ReturnsProductStatistics_14")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var productService = new ProductService(context, _mapper, _mockLogger.Object);
            
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "In Stock Product", Price = 100, Stock = 10 },
                new Product { Id = 2, Name = "Out of Stock Product", Price = 200, Stock = 0 }
            };
            
            context.Products.AddRange(products);
            await context.SaveChangesAsync();

            // Act
            var result = await productService.GetProductStatisticsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalProducts);
            Assert.Equal(1, result.TotalInStock);
            Assert.Equal(1, result.TotalOutOfStock);
            Assert.Equal(150, result.AveragePrice);
        }
    }
}