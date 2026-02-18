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
    public class FavoriteServiceTests
    {
        private readonly Mock<ILogger<FavoriteService>> _mockLogger;
        private readonly IMapper _mapper;
        private readonly DbContextOptions<OnlineStoreDbContext> _options;

        public FavoriteServiceTests()
        {
            _mockLogger = new Mock<ILogger<FavoriteService>>();
            
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<FavoriteItem, FavoriteListItemDto>();
                cfg.CreateMap<FavoriteItem, FavoriteResponseDto>();
                cfg.CreateMap<Product, ProductResponseDto>();
                cfg.CreateMap<Product, ProductListItemDto>();
            });
            _mapper = config.CreateMapper();
            
            _options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "FavoriteServiceTestDb")
                .Options;
        }

        [Fact]
        public async Task GetUserFavoritesAsync_WhenFavoritesExist_ReturnsFavoritesList()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "GetUserFavoritesAsync_WhenFavoritesExist_ReturnsFavoritesList_1")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var favoriteService = new FavoriteService(context, _mapper, _mockLogger.Object);
            
            var product = new Product { Id = 1, Name = "Test Product", Price = 100 };
            var favorite = new FavoriteItem { Id = 1, UserId = 1, ProductId = 1, Product = product };
            
            context.Products.Add(product);
            context.FavoriteItems.Add(favorite);
            await context.SaveChangesAsync();

            // Act
            var result = await favoriteService.GetUserFavoritesAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(1, result.First().ProductId);
        }

        [Fact]
        public async Task GetUserFavoritesAsync_WhenNoFavoritesExist_ReturnsEmptyList()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "GetUserFavoritesAsync_WhenNoFavoritesExist_ReturnsEmptyList_2")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var favoriteService = new FavoriteService(context, _mapper, _mockLogger.Object);

            // Act
            var result = await favoriteService.GetUserFavoritesAsync(999);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetUserFavoritesAsync_WithPaging_ReturnsPagedResult()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "GetUserFavoritesAsync_WithPaging_ReturnsPagedResult_3")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var favoriteService = new FavoriteService(context, _mapper, _mockLogger.Object);
            
            var product = new Product { Id = 1, Name = "Test Product", Price = 100 };
            context.Products.Add(product);
            
            var favorites = new List<FavoriteItem>();
            for (int i = 1; i <= 15; i++)
            {
                favorites.Add(new FavoriteItem { Id = i, UserId = 1, ProductId = 1, Product = product });
            }
            
            context.FavoriteItems.AddRange(favorites);
            await context.SaveChangesAsync();

            // Act
            var result = await favoriteService.GetUserFavoritesAsync(1, 1, 10);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(10, result.Items.Count());
            Assert.Equal(1, result.PageNumber);
            Assert.Equal(10, result.PageSize);
            Assert.Equal(2, result.TotalPages);
            Assert.Equal(15, result.TotalCount);
        }

        [Fact]
        public async Task AddFavoriteAsync_WhenProductExistsAndNotAlreadyFavorite_AddsFavorite()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "AddFavoriteAsync_WhenProductExistsAndNotAlreadyFavorite_AddsFavorite_4")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var favoriteService = new FavoriteService(context, _mapper, _mockLogger.Object);
            
            var product = new Product { Id = 1, Name = "Test Product", Price = 100 };
            context.Products.Add(product);
            await context.SaveChangesAsync();
            
            var favoriteDto = new FavoriteCreateDto
            {
                UserId = 1,
                ProductId = 1
            };

            // Act
            var result = await favoriteService.AddFavoriteAsync(favoriteDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.ProductId);
            Assert.Equal(1, context.FavoriteItems.Count());
        }

        [Fact]
        public async Task AddFavoriteAsync_WhenProductDoesNotExist_ThrowsKeyNotFoundException()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "AddFavoriteAsync_WhenProductDoesNotExist_ThrowsKeyNotFoundException_5")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var favoriteService = new FavoriteService(context, _mapper, _mockLogger.Object);
            
            var favoriteDto = new FavoriteCreateDto
            {
                UserId = 1,
                ProductId = 999
            };

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => favoriteService.AddFavoriteAsync(favoriteDto));
        }

        [Fact]
        public async Task AddFavoriteAsync_WhenProductAlreadyFavorite_ThrowsInvalidOperationException()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "AddFavoriteAsync_WhenProductAlreadyFavorite_ThrowsInvalidOperationException_6")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var favoriteService = new FavoriteService(context, _mapper, _mockLogger.Object);
            
            var product = new Product { Id = 1, Name = "Test Product", Price = 100 };
            var favorite = new FavoriteItem { Id = 1, UserId = 1, ProductId = 1, Product = product };
            
            context.Products.Add(product);
            context.FavoriteItems.Add(favorite);
            await context.SaveChangesAsync();
            
            var favoriteDto = new FavoriteCreateDto
            {
                UserId = 1,
                ProductId = 1
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => favoriteService.AddFavoriteAsync(favoriteDto));
        }

        [Fact]
        public async Task RemoveFavoriteAsync_WhenFavoriteExists_RemovesFavorite()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "RemoveFavoriteAsync_WhenFavoriteExists_RemovesFavorite_7")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var favoriteService = new FavoriteService(context, _mapper, _mockLogger.Object);
            
            var product = new Product { Id = 1, Name = "Test Product", Price = 100 };
            var favorite = new FavoriteItem { Id = 1, UserId = 1, ProductId = 1, Product = product };
            
            context.Products.Add(product);
            context.FavoriteItems.Add(favorite);
            await context.SaveChangesAsync();

            // Act
            var result = await favoriteService.RemoveFavoriteAsync(1, 1);

            // Assert
            Assert.True(result);
            Assert.Equal(0, context.FavoriteItems.Count());
        }

        [Fact]
        public async Task RemoveFavoriteAsync_WhenFavoriteDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "RemoveFavoriteAsync_WhenFavoriteDoesNotExist_ReturnsFalse_8")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var favoriteService = new FavoriteService(context, _mapper, _mockLogger.Object);

            // Act
            var result = await favoriteService.RemoveFavoriteAsync(999, 999);

            // Assert
            Assert.False(result);
        }
    }
}