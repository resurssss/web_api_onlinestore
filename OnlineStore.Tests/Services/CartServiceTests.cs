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
    public class CartServiceTests
    {
        private readonly Mock<ILogger<CartService>> _mockLogger;
        private readonly IMapper _mapper;
        private readonly DbContextOptions<OnlineStoreDbContext> _options;

        public CartServiceTests()
        {
            _mockLogger = new Mock<ILogger<CartService>>();
            
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<Cart, CartResponseDto>();
                cfg.CreateMap<CartItem, CartItemResponseDto>();
                cfg.CreateMap<Product, ProductResponseDto>();
                cfg.CreateMap<Coupon, CouponResponseDto>();
            });
            _mapper = config.CreateMapper();
            
            _options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "CartServiceTestDb")
                .Options;
        }

        [Fact]
        public async Task GetCartAsync_WhenCartExists_ReturnsCartResponseDto()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "GetCartAsync_WhenCartExists_ReturnsCartResponseDto_1")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var cartService = new CartService(context, _mapper, _mockLogger.Object);
            
            var cart = new Cart { Id = 101, UserId = 101 };
            context.Carts.Add(cart);
            await context.SaveChangesAsync();

            // Act
            var result = await cartService.GetCartAsync(101);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(101, result.Id);
        }

        [Fact]
        public async Task GetCartAsync_WhenCartDoesNotExist_ThrowsKeyNotFoundException()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "GetCartAsync_WhenCartDoesNotExist_ThrowsKeyNotFoundException_2")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var cartService = new CartService(context, _mapper, _mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => cartService.GetCartAsync(999));
        }

        [Fact]
        public async Task AddItemAsync_WhenProductExistsAndHasStock_AddsItemToCart()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "AddItemAsync_WhenProductExistsAndHasStock_AddsItemToCart")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var cartService = new CartService(context, _mapper, _mockLogger.Object);
            
            var cart = new Cart { Id = 102, UserId = 102 };
            var product = new Product { Id = 102, Name = "Test Product", Price = 100, Stock = 10 };
            
            context.Carts.Add(cart);
            context.Products.Add(product);
            await context.SaveChangesAsync();

            // Act
            var result = await cartService.AddItemAsync(102, 102, 2);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Items);
            Assert.Equal(2, result.Items.First().Quantity);
            Assert.Equal(8, context.Products.First(p => p.Id == 102).Stock);
        }

        [Fact]
        public async Task AddItemAsync_WhenProductDoesNotExist_ThrowsKeyNotFoundException()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "AddItemAsync_WhenProductDoesNotExist_ThrowsKeyNotFoundException")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var cartService = new CartService(context, _mapper, _mockLogger.Object);
            
            var cart = new Cart { Id = 103, UserId = 103 };
            context.Carts.Add(cart);
            await context.SaveChangesAsync();

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => cartService.AddItemAsync(103, 999, 1));
        }

        [Fact]
        public async Task AddItemAsync_WhenNotEnoughStock_ThrowsInvalidOperationException()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "AddItemAsync_WhenNotEnoughStock_ThrowsInvalidOperationException_3")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var cartService = new CartService(context, _mapper, _mockLogger.Object);
            
            var cart = new Cart { Id = 104, UserId = 104 };
            var product = new Product { Id = 104, Name = "Test Product", Price = 100, Stock = 5 };
            
            context.Carts.Add(cart);
            context.Products.Add(product);
            await context.SaveChangesAsync();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => cartService.AddItemAsync(104, 104, 10));
        }

        [Fact]
        public async Task UpdateItemQuantityAsync_WhenItemExistsAndHasStock_UpdatesQuantity()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "UpdateItemQuantityAsync_WhenItemExistsAndHasStock_UpdatesQuantity_4")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var cartService = new CartService(context, _mapper, _mockLogger.Object);
            
            var product = new Product { Id = 105, Name = "Test Product", Price = 100, Stock = 10 };
            var cart = new Cart { Id = 105, UserId = 105 };
            var cartItem = new CartItem { CartId = 105, ProductId = 105, Quantity = 2, UnitPrice = 100 };
            
            context.Products.Add(product);
            context.Carts.Add(cart);
            context.CartItems.Add(cartItem);
            await context.SaveChangesAsync();

            // Act
            var result = await cartService.UpdateItemQuantityAsync(105, 105, 5);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(5, result.Items.First().Quantity);
            Assert.Equal(7, context.Products.First(p => p.Id == 105).Stock);
        }

        [Fact]
        public async Task UpdateItemQuantityAsync_WhenItemDoesNotExist_ThrowsKeyNotFoundException()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "UpdateItemQuantityAsync_WhenItemDoesNotExist_ThrowsKeyNotFoundException_5")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var cartService = new CartService(context, _mapper, _mockLogger.Object);
            
            var cart = new Cart { Id = 100, UserId = 100 };
            context.Carts.Add(cart);
            await context.SaveChangesAsync();

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => cartService.UpdateItemQuantityAsync(100, 999, 5));
        }

        [Fact]
        public async Task RemoveItemAsync_WhenItemExists_RemovesItemFromCart()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "RemoveItemAsync_WhenItemExists_RemovesItemFromCart_6")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var cartService = new CartService(context, _mapper, _mockLogger.Object);
            
            var product = new Product { Id = 106, Name = "Test Product", Price = 100, Stock = 10 };
            var cart = new Cart { Id = 106, UserId = 106 };
            var cartItem = new CartItem { CartId = 106, ProductId = 106, Quantity = 2, UnitPrice = 100 };
            
            context.Products.Add(product);
            context.Carts.Add(cart);
            context.CartItems.Add(cartItem);
            await context.SaveChangesAsync();

            // Act
            await cartService.RemoveItemAsync(106, 106);

            // Assert
            var updatedCart = await cartService.GetCartAsync(106);
            Assert.Empty(updatedCart.Items);
            Assert.Equal(12, context.Products.First(p => p.Id == 106).Stock);
        }

        [Fact]
        public async Task ClearCartAsync_WhenCartExists_ClearsAllItems()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "ClearCartAsync_WhenCartExists_ClearsAllItems_7")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var cartService = new CartService(context, _mapper, _mockLogger.Object);
            
            var product1 = new Product { Id = 107, Name = "Test Product 1", Price = 100, Stock = 10 };
            var product2 = new Product { Id = 108, Name = "Test Product 2", Price = 200, Stock = 5 };
            var cart = new Cart { Id = 107, UserId = 107 };
            var cartItem1 = new CartItem { CartId = 107, ProductId = 107, Quantity = 2, UnitPrice = 100 };
            var cartItem2 = new CartItem { CartId = 107, ProductId = 108, Quantity = 1, UnitPrice = 200 };
            
            context.Products.AddRange(product1, product2);
            context.Carts.Add(cart);
            context.CartItems.AddRange(cartItem1, cartItem2);
            await context.SaveChangesAsync();

            // Act
            var result = await cartService.ClearCartAsync(107);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Items);
            Assert.Equal(12, context.Products.First(p => p.Id == 107).Stock);
            Assert.Equal(6, context.Products.First(p => p.Id == 108).Stock);
        }

        [Fact]
        public async Task ApplyCouponAsync_WhenValidCouponExists_AppliesCouponToCart()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "ApplyCouponAsync_WhenValidCouponExists_AppliesCouponToCart_8")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var cartService = new CartService(context, _mapper, _mockLogger.Object);
            
            var cart = new Cart { Id = 108, UserId = 108 };
            var coupon = new Coupon
            {
                Id = 108,
                Code = "TEST10",
                DiscountPercent = 10,
                IsActive = true,
                ExpirationDate = DateTime.UtcNow.AddDays(30),
                UsageLimit = 100,
                TimesUsed = 0
            };
            
            context.Carts.Add(cart);
            context.Coupons.Add(coupon);
            await context.SaveChangesAsync();

            // Act
            var result = await cartService.ApplyCouponAsync(108, "TEST10");

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.AppliedCoupon);
            Assert.Equal(1, context.Coupons.First(c => c.Id == 108).TimesUsed);
        }

        [Fact]
        public async Task ApplyCouponAsync_WhenCouponDoesNotExist_ThrowsKeyNotFoundException()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "ApplyCouponAsync_WhenCouponDoesNotExist_ThrowsKeyNotFoundException_9")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var cartService = new CartService(context, _mapper, _mockLogger.Object);
            
            var cart = new Cart { Id = 109, UserId = 109 };
            context.Carts.Add(cart);
            await context.SaveChangesAsync();

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => cartService.ApplyCouponAsync(109, "INVALID"));
        }

        [Fact]
        public async Task ApplyCouponAsync_WhenCouponExpired_ThrowsInvalidOperationException()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "ApplyCouponAsync_WhenCouponExpired_ThrowsInvalidOperationException_10")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var cartService = new CartService(context, _mapper, _mockLogger.Object);
            
            var cart = new Cart { Id = 110, UserId = 110 };
            var expiredCoupon = new Coupon
            {
                Id = 110,
                Code = "EXPIRED",
                DiscountPercent = 10,
                IsActive = true,
                ExpirationDate = DateTime.UtcNow.AddDays(-1),
                UsageLimit = 100,
                TimesUsed = 0
            };
            
            context.Carts.Add(cart);
            context.Coupons.Add(expiredCoupon);
            await context.SaveChangesAsync();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => cartService.ApplyCouponAsync(110, "EXPIRED"));
        }
    }
}