using FluentValidation.TestHelper;
using Moq;
using OnlineStore.Core.Validators;
using OnlineStore.Core.DTOs;
using OnlineStore.Core.Interfaces;
using OnlineStore.Core.Models;
using Xunit;

namespace OnlineStore.Tests.Validators
{
    public class FavoriteValidatorTests
    {
        private readonly FavoriteValidator _validator;

        public FavoriteValidatorTests()
        {
            _validator = new FavoriteValidator();
        }

        [Fact]
        public async Task Should_have_error_when_UserId_is_empty()
        {
            var model = new FavoriteCreateDto
            {
                UserId = 0,
                ProductId = 1
            };
            var result = await _validator.TestValidateAsync(model);
            result.ShouldHaveValidationErrorFor(x => x.UserId);
        }

        [Fact]
        public async Task Should_not_have_error_when_UserId_is_valid()
        {
            var model = new FavoriteCreateDto
            {
                UserId = 1,
                ProductId = 1
            };
            var result = await _validator.TestValidateAsync(model);
            result.ShouldNotHaveValidationErrorFor(x => x.UserId);
        }

        [Fact]
        public async Task Should_have_error_when_ProductId_is_empty()
        {
            var model = new FavoriteCreateDto
            {
                UserId = 1,
                ProductId = 0
            };
            var result = await _validator.TestValidateAsync(model);
            result.ShouldHaveValidationErrorFor(x => x.ProductId);
        }

        [Fact]
        public async Task Should_not_have_error_when_ProductId_is_valid()
        {
            // В новом валидаторе проверка существования продукта происходит на уровне сервиса
            // Валидатор только проверяет, что ProductId не пустой

            var model = new FavoriteCreateDto
            {
                UserId = 1,
                ProductId = 1
            };
            var result = await _validator.TestValidateAsync(model);
            result.ShouldNotHaveValidationErrorFor(x => x.ProductId);
        }

    }
}