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
    public class ReviewServiceTests
    {
        private readonly Mock<ILogger<ReviewService>> _mockLogger;
        private readonly IMapper _mapper;
        private readonly DbContextOptions<OnlineStoreDbContext> _options;

        public ReviewServiceTests()
        {
            _mockLogger = new Mock<ILogger<ReviewService>>();
            
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<Review, ReviewResponseDto>();
                cfg.CreateMap<ReviewCreateDto, Review>();
            });
            _mapper = config.CreateMapper();
            
            _options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "ReviewServiceTestDb")
                .Options;
        }

        [Fact]
        public async Task GetReviewsAsync_WhenReviewsExist_ReturnsReviews()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "GetReviewsAsync_WhenReviewsExist_ReturnsReviews")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var reviewService = new ReviewService(context, _mapper, _mockLogger.Object);
            
            var product = new Product { Id = 203, Name = "Test Product", Price = 100 };
            var reviews = new List<Review>
            {
                new Review { Id = 202, ProductId = 203, Author = "User1", Rating = 5, Comment = "Great product!" },
                new Review { Id = 203, ProductId = 203, Author = "User2", Rating = 4, Comment = "Good product" }
            };
            
            context.Products.Add(product);
            context.Reviews.AddRange(reviews);
            await context.SaveChangesAsync();

            // Act
            var result = await reviewService.GetReviewsAsync(203);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetReviewsAsync_WhenNoReviewsExist_ReturnsEmptyList()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "GetReviewsAsync_WhenNoReviewsExist_ReturnsEmptyList")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var reviewService = new ReviewService(context, _mapper, _mockLogger.Object);

            // Act
            var result = await reviewService.GetReviewsAsync(999);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetReviewsAsync_WithPaging_ReturnsPagedResult()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "GetReviewsAsync_WithPaging_ReturnsPagedResult")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var reviewService = new ReviewService(context, _mapper, _mockLogger.Object);
            
            var product = new Product { Id = 204, Name = "Test Product", Price = 100 };
            context.Products.Add(product);
            
            var reviews = new List<Review>();
            for (int i = 1; i <= 15; i++)
            {
                reviews.Add(new Review { Id = 203 + i, ProductId = 204, Author = $"User{i}", Rating = 5, Comment = $"Comment {i}" });
            }
            
            context.Reviews.AddRange(reviews);
            await context.SaveChangesAsync();

            // Act
            var result = await reviewService.GetReviewsAsync(204, 1, 10);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(10, result.Items.Count());
            Assert.Equal(1, result.PageNumber);
            Assert.Equal(10, result.PageSize);
            Assert.Equal(2, result.TotalPages);
            Assert.Equal(15, result.TotalCount);
        }

        [Fact]
        public async Task AddReviewAsync_WhenProductExistsAndUserHasNotReviewed_AddsReview()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "AddReviewAsync_WhenProductExistsAndUserHasNotReviewed_AddsReview")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var reviewService = new ReviewService(context, _mapper, _mockLogger.Object);
            
            var product = new Product { Id = 201, Name = "Test Product", Price = 100 };
            context.Products.Add(product);
            await context.SaveChangesAsync();
            
            var reviewDto = new ReviewCreateDto
            {
                ProductId = 201,
                Author = "User1",
                Rating = 5,
                Comment = "Great product!"
            };

            // Act
            var result = await reviewService.AddReviewAsync(reviewDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("User1", result.Author);
            Assert.Equal(5, result.Rating);
            Assert.Equal(1, context.Reviews.Count());
        }

        [Fact]
        public async Task AddReviewAsync_WhenProductDoesNotExist_ThrowsKeyNotFoundException()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "AddReviewAsync_WhenProductDoesNotExist_ThrowsKeyNotFoundException")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var reviewService = new ReviewService(context, _mapper, _mockLogger.Object);
            
            var reviewDto = new ReviewCreateDto
            {
                ProductId = 999,
                Author = "User1",
                Rating = 5,
                Comment = "Great product!"
            };

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => reviewService.AddReviewAsync(reviewDto));
        }

        [Fact]
        public async Task AddReviewAsync_WhenUserAlreadyReviewed_ThrowsInvalidOperationException()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "AddReviewAsync_WhenUserAlreadyReviewed_ThrowsInvalidOperationException")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var reviewService = new ReviewService(context, _mapper, _mockLogger.Object);
            
            var product = new Product { Id = 202, Name = "Test Product", Price = 100 };
            var existingReview = new Review { Id = 201, ProductId = 202, Author = "User1", Rating = 5, Comment = "Great product!" };
            
            context.Products.Add(product);
            context.Reviews.Add(existingReview);
            await context.SaveChangesAsync();
            
            var reviewDto = new ReviewCreateDto
            {
                ProductId = 202,
                Author = "User1",
                Rating = 4,
                Comment = "Good product"
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => reviewService.AddReviewAsync(reviewDto));
        }

        [Fact]
        public async Task GetProductRatingAsync_WhenReviewsExist_ReturnsRating()
        {
            // Arrange
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "GetProductRatingAsync_WhenReviewsExist_ReturnsRating")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var reviewService = new ReviewService(context, _mapper, _mockLogger.Object);
            
            var product = new Product { Id = 1, Name = "Test Product", Price = 100 };
            var reviews = new List<Review>
            {
                new Review { Id = 1, ProductId = 1, Author = "User1", Rating = 5 },
                new Review { Id = 2, ProductId = 1, Author = "User2", Rating = 3 }
            };
            
            context.Products.Add(product);
            context.Reviews.AddRange(reviews);
            await context.SaveChangesAsync();

            // Act
            var (rating, count) = await reviewService.GetProductRatingAsync(1);

            // Assert
            Assert.Equal(4.0, rating);
            Assert.Equal(2, count);
        }

        [Fact]
        public async Task GetProductRatingAsync_WhenNoReviewsExist_ReturnsZeroRating()
        {
            // Arrange
            // Arrange
            var options = new DbContextOptionsBuilder<OnlineStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "GetProductRatingAsync_WhenNoReviewsExist_ReturnsZeroRating")
                .Options;
            using var context = new OnlineStoreDbContext(options);
            var reviewService = new ReviewService(context, _mapper, _mockLogger.Object);

            // Act
            var (rating, count) = await reviewService.GetProductRatingAsync(999);

            // Assert
            Assert.Equal(0.0, rating);
            Assert.Equal(0, count);
        }
    }
}