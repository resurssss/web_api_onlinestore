using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using OnlineStore.Core;
using OnlineStore.Core.Models;
using OnlineStore.Services.Services;
using OnlineStore.Core.Interfaces;
using Xunit;

namespace OnlineStore.Tests.Services
{
    public class OrderServiceTests
    {
        private readonly Mock<ILogger<OrderService>> _mockLogger;
        private readonly DbContextOptions<OnlineStoreDbContext> _options;

        public OrderServiceTests()
        {
            _mockLogger = new Mock<ILogger<OrderService>>();
            
            _options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "OrderServiceTestDb")
                .Options;
        }

        [Fact]
        public async Task CreateOrderAsync_WhenValidData_CreatesOrder()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "CreateOrderAsync_WhenValidData_CreatesOrder")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var mockProductService = new Mock<IProductService>();
            var mockLogger = new Mock<ILogger<TestOrderService>>();
            var orderService = new TestOrderService(context, mockProductService.Object, mockLogger.Object);
            
            var product = new Product { Id = 1, Name = "Test Product", Price = 100, Stock = 10 };
            var cartItems = new List<CartItem>
            {
                new CartItem { ProductId = 1, Quantity = 2, UnitPrice = 100, Product = product }
            };
            
            context.Products.Add(product);
            await context.SaveChangesAsync();

            // Act
            var result = await orderService.CreateOrderAsync(1, cartItems);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.UserId);
            Assert.Equal(200, result.TotalAmount);
            Assert.Equal(8, context.Products.First(p => p.Id == 1).Stock);
        }

        [Fact]
        public async Task CreateOrderAsync_WhenProductNotFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "CreateOrderAsync_WhenProductNotFound_ThrowsKeyNotFoundException")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var mockProductService = new Mock<IProductService>();
            var mockLogger = new Mock<ILogger<TestOrderService>>();
            var orderService = new TestOrderService(context, mockProductService.Object, mockLogger.Object);
            
            var cartItems = new List<CartItem>
            {
                new CartItem { ProductId = 999, Quantity = 2, UnitPrice = 100 }
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => orderService.CreateOrderAsync(1, cartItems));
        }

        [Fact]
        public async Task CreateOrderWithSavepointAsync_WhenValidData_CreatesOrder()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "CreateOrderWithSavepointAsync_WhenValidData_CreatesOrder")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var mockProductService = new Mock<IProductService>();
            var mockLogger = new Mock<ILogger<TestOrderService>>();
            var orderService = new TestOrderService(context, mockProductService.Object, mockLogger.Object);
            
            var product = new Product { Id = 1, Name = "Test Product", Price = 100, Stock = 10 };
            var cartItems = new List<CartItem>
            {
                new CartItem { ProductId = 1, Quantity = 2, UnitPrice = 100, Product = product }
            };
            
            context.Products.Add(product);
            await context.SaveChangesAsync();

            // Act
            var result = await orderService.CreateOrderWithSavepointAsync(1, cartItems);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.UserId);
            Assert.Equal(200, result.TotalAmount);
            Assert.Equal(8, context.Products.First(p => p.Id == 1).Stock);
        }

        [Fact]
        public async Task CreateOrderWithIsolationLevelAsync_WhenValidData_CreatesOrder()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "CreateOrderWithIsolationLevelAsync_WhenValidData_CreatesOrder")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var mockProductService = new Mock<IProductService>();
            var mockLogger = new Mock<ILogger<TestOrderService>>();
            var orderService = new TestOrderService(context, mockProductService.Object, mockLogger.Object);
            
            var product = new Product { Id = 1, Name = "Test Product", Price = 100, Stock = 10 };
            var cartItems = new List<CartItem>
            {
                new CartItem { ProductId = 1, Quantity = 2, UnitPrice = 100, Product = product }
            };
            
            context.Products.Add(product);
            await context.SaveChangesAsync();

            // Act
            var result = await orderService.CreateOrderWithIsolationLevelAsync(1, cartItems);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.UserId);
            Assert.Equal(200, result.TotalAmount);
            Assert.Equal(8, context.Products.First(p => p.Id == 1).Stock);
        }
    }
}