using FluentValidation.TestHelper;
using OnlineStore.Core.Validators;
using OnlineStore.Core.DTOs;
using Xunit;

namespace OnlineStore.Tests.Validators
{
    public class CartItemCreateValidatorTests
    {
        private readonly CartItemCreateValidator _validator;

        public CartItemCreateValidatorTests()
        {
            _validator = new CartItemCreateValidator();
        }

        [Fact]
        public void Should_have_error_when_ProductId_is_empty()
        {
            var model = new CartItemCreateDto { ProductId = 0, Quantity = 1 };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.ProductId);
        }

        [Fact]
        public void Should_not_have_error_when_ProductId_is_specified()
        {
            var model = new CartItemCreateDto { ProductId = 1, Quantity = 1 };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.ProductId);
        }

        [Fact]
        public void Should_have_error_when_Quantity_is_zero()
        {
            var model = new CartItemCreateDto { ProductId = 1, Quantity = 0 };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Quantity);
        }

        [Fact]
        public void Should_have_error_when_Quantity_is_negative()
        {
            var model = new CartItemCreateDto { ProductId = 1, Quantity = -1 };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Quantity);
        }

        [Fact]
        public void Should_not_have_error_when_Quantity_is_positive()
        {
            var model = new CartItemCreateDto { ProductId = 1, Quantity = 1 };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Quantity);
        }
    }

    public class CartItemUpdateValidatorTests
    {
        private readonly CartItemUpdateValidator _validator;

        public CartItemUpdateValidatorTests()
        {
            _validator = new CartItemUpdateValidator();
        }

        [Fact]
        public void Should_have_error_when_Quantity_is_zero()
        {
            var model = new CartItemUpdateDto { Quantity = 0 };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Quantity);
        }

        [Fact]
        public void Should_have_error_when_Quantity_is_negative()
        {
            var model = new CartItemUpdateDto { Quantity = -1 };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Quantity);
        }

        [Fact]
        public void Should_not_have_error_when_Quantity_is_positive()
        {
            var model = new CartItemUpdateDto { Quantity = 1 };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Quantity);
        }
    }
}