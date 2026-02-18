using FluentValidation.TestHelper;
using Moq;
using OnlineStore.Core.Validators;
using OnlineStore.Core.DTOs;
using OnlineStore.Core.Interfaces;
using OnlineStore.Core.Models;
using Xunit;

namespace OnlineStore.Tests.Validators
{
    public class ReviewValidatorTests
    {
        private readonly ReviewValidator _validator;

        public ReviewValidatorTests()
        {
            _validator = new ReviewValidator();
        }

        [Fact]
        public async Task Should_have_error_when_ProductId_is_empty()
        {
            var model = new ReviewCreateDto
            {
                ProductId = 0,
                Author = "Test Author",
                Rating = 5,
                Comment = "Test comment"
            };
            var result = await _validator.TestValidateAsync(model);
            result.ShouldHaveValidationErrorFor(x => x.ProductId);
        }

        [Fact]
        public async Task Should_not_have_error_when_ProductId_is_valid()
        {
            // В новом валидаторе проверка существования продукта происходит на уровне сервиса
            // Валидатор только проверяет, что ProductId не пустой

            var model = new ReviewCreateDto
            {
                ProductId = 1,
                Author = "Test Author",
                Rating = 5,
                Comment = "Test comment"
            };
            var result = await _validator.TestValidateAsync(model);
            result.ShouldNotHaveValidationErrorFor(x => x.ProductId);
        }

        [Fact]
        public async Task Should_have_error_when_Author_is_empty()
        {
            var model = new ReviewCreateDto
            {
                ProductId = 1,
                Author = "",
                Rating = 5,
                Comment = "Test comment"
            };
            var result = await _validator.TestValidateAsync(model);
            result.ShouldHaveValidationErrorFor(x => x.Author);
        }

        [Fact]
        public async Task Should_have_error_when_Author_is_too_long()
        {
            var model = new ReviewCreateDto
            {
                ProductId = 1,
                Author = new string('A', 101), // 101 символ
                Rating = 5,
                Comment = "Test comment"
            };
            var result = await _validator.TestValidateAsync(model);
            result.ShouldHaveValidationErrorFor(x => x.Author);
        }

        [Fact]
        public async Task Should_not_have_error_when_Author_is_valid()
        {
            var model = new ReviewCreateDto
            {
                ProductId = 1,
                Author = "Valid Author",
                Rating = 5,
                Comment = "Test comment"
            };
            var result = await _validator.TestValidateAsync(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Author);
        }

        [Fact]
        public async Task Should_have_error_when_Rating_is_less_than_1()
        {
            var model = new ReviewCreateDto
            {
                ProductId = 1,
                Author = "Test Author",
                Rating = 0,
                Comment = "Test comment"
            };
            var result = await _validator.TestValidateAsync(model);
            result.ShouldHaveValidationErrorFor(x => x.Rating);
        }

        [Fact]
        public async Task Should_have_error_when_Rating_is_greater_than_5()
        {
            var model = new ReviewCreateDto
            {
                ProductId = 1,
                Author = "Test Author",
                Rating = 6,
                Comment = "Test comment"
            };
            var result = await _validator.TestValidateAsync(model);
            result.ShouldHaveValidationErrorFor(x => x.Rating);
        }

        [Fact]
        public async Task Should_not_have_error_when_Rating_is_valid()
        {
            var model = new ReviewCreateDto
            {
                ProductId = 1,
                Author = "Test Author",
                Rating = 5,
                Comment = "Test comment"
            };
            var result = await _validator.TestValidateAsync(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Rating);
        }

        [Fact]
        public async Task Should_have_error_when_Comment_is_too_long()
        {
            var model = new ReviewCreateDto
            {
                ProductId = 1,
                Author = "Test Author",
                Rating = 5,
                Comment = new string('A', 1001) // 1001 символ
            };
            var result = await _validator.TestValidateAsync(model);
            result.ShouldHaveValidationErrorFor(x => x.Comment);
        }

        [Fact]
        public async Task Should_not_have_error_when_Comment_is_valid()
        {
            var model = new ReviewCreateDto
            {
                ProductId = 1,
                Author = "Test Author",
                Rating = 5,
                Comment = "Valid comment"
            };
            var result = await _validator.TestValidateAsync(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Comment);
        }

    }

    public class ReviewUpdateValidatorTests
    {
        private readonly ReviewUpdateValidator _validator;

        public ReviewUpdateValidatorTests()
        {
            _validator = new ReviewUpdateValidator();
        }

        [Fact]
        public void Should_have_error_when_Rating_is_less_than_1()
        {
            var model = new ReviewUpdateDto
            {
                Rating = 0,
            };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Rating);
        }

        [Fact]
        public void Should_have_error_when_Rating_is_greater_than_5()
        {
            var model = new ReviewUpdateDto
            {
                Rating = 6,
            };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Rating);
        }

        [Fact]
        public void Should_not_have_error_when_Rating_is_valid()
        {
            var model = new ReviewUpdateDto
            {
                Rating = 5,
            };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Rating);
        }

        [Fact]
        public void Should_not_have_error_when_Rating_is_null()
        {
            var model = new ReviewUpdateDto
            {
                Rating = null,
            };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Rating);
        }

        [Fact]
        public void Should_have_error_when_Comment_is_too_long()
        {
            var model = new ReviewUpdateDto
            {
                Comment = new string('A', 1001), // 1001 символ
            };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Comment);
        }

        [Fact]
        public void Should_not_have_error_when_Comment_is_valid()
        {
            var model = new ReviewUpdateDto
            {
                Comment = "Valid comment",
            };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Comment);
        }

        [Fact]
        public void Should_not_have_error_when_Comment_is_null()
        {
            var model = new ReviewUpdateDto
            {
                Comment = null,
            };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Comment);
        }
    }
}
