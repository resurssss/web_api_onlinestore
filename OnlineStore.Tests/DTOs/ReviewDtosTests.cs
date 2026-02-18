using OnlineStore.Core.DTOs;
using Xunit;

namespace OnlineStore.Tests.DTOs
{
    public class ReviewDtosTests
    {
        [Fact]
        public void ReviewCreateDto_Should_Create_Instance_With_Correct_Properties()
        {
            // Arrange
            var productId = 1;
            var author = "Test Author";
            var rating = 5;
            var comment = "Test Comment";

            // Act
            var dto = new ReviewCreateDto
            {
                ProductId = productId,
                Author = author,
                Rating = rating,
                Comment = comment
            };

            // Assert
            Assert.Equal(productId, dto.ProductId);
            Assert.Equal(author, dto.Author);
            Assert.Equal(rating, dto.Rating);
            Assert.Equal(comment, dto.Comment);
        }

        [Fact]
        public void ReviewUpdateDto_Should_Create_Instance_With_Correct_Properties()
        {
            // Arrange
            var rating = 4;
            var comment = "Updated Comment";

            // Act
            var dto = new ReviewUpdateDto
            {
                Rating = rating,
                Comment = comment
            };

            // Assert
            Assert.Equal(rating, dto.Rating);
            Assert.Equal(comment, dto.Comment);
        }

        [Fact]
        public void ReviewResponseDto_Should_Create_Instance_With_Correct_Properties()
        {
            // Arrange
            var id = 1;
            var productId = 1;
            var author = "Test Author";
            var rating = 5;
            var comment = "Test Comment";
            var createdAt = DateTime.UtcNow;
            var updatedAt = DateTime.UtcNow;

            // Act
            var dto = new ReviewResponseDto
            {
                Id = id,
                ProductId = productId,
                Author = author,
                Rating = rating,
                Comment = comment,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt
            };

            // Assert
            Assert.Equal(id, dto.Id);
            Assert.Equal(productId, dto.ProductId);
            Assert.Equal(author, dto.Author);
            Assert.Equal(rating, dto.Rating);
            Assert.Equal(comment, dto.Comment);
            Assert.Equal(createdAt, dto.CreatedAt);
            Assert.Equal(updatedAt, dto.UpdatedAt);
        }

        [Fact]
        public void ReviewListItemDto_Should_Create_Instance_With_Correct_Properties()
        {
            // Arrange
            var id = 1;
            var author = "Test Author";
            var rating = 5;
            var commentPreview = "Test Comment";
            var createdAt = DateTime.UtcNow;

            // Act
            var dto = new ReviewListItemDto
            {
                Id = id,
                Author = author,
                Rating = rating,
                CommentPreview = commentPreview,
                CreatedAt = createdAt
            };

            // Assert
            Assert.Equal(id, dto.Id);
            Assert.Equal(author, dto.Author);
            Assert.Equal(rating, dto.Rating);
            Assert.Equal(commentPreview, dto.CommentPreview);
            Assert.Equal(createdAt, dto.CreatedAt);
        }
    }
}