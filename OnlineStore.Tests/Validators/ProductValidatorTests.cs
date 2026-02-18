using FluentValidation.TestHelper;
using OnlineStore.Core.Validators;
using OnlineStore.Core.DTOs;
using Xunit;

namespace OnlineStore.Tests.Validators
{
    public class ProductCreateValidatorTests
    {
        private readonly ProductCreateValidator _validator;

        public ProductCreateValidatorTests()
        {
            _validator = new ProductCreateValidator();
        }

        [Fact]
        public void Should_have_error_when_Name_is_empty()
        {
            var model = new ProductCreateDto
            {
                Name = "",
                Description = "Test Description",
                Price = 10,
                Stock = 5
            };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_have_error_when_Name_is_too_long()
        {
            var model = new ProductCreateDto
            {
                Name = new string('A', 201), // 201 символ
                Description = "Test Description",
                Price = 10,
                Stock = 5
            };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_not_have_error_when_Name_is_valid()
        {
            var model = new ProductCreateDto
            {
                Name = "Valid Product Name",
                Description = "Test Description",
                Price = 10,
                Stock = 5
            };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_have_error_when_Description_is_empty()
        {
            var model = new ProductCreateDto
            {
                Name = "Valid Product Name",
                Description = "",
                Price = 10,
                Stock = 5
            };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_have_error_when_Description_is_too_long()
        {
            var model = new ProductCreateDto
            {
                Name = "Valid Product Name",
                Description = new string('A', 1001), // 1001 символ
                Price = 10,
                Stock = 5
            };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_not_have_error_when_Description_is_valid()
        {
            var model = new ProductCreateDto
            {
                Name = "Valid Product Name",
                Description = "Valid description",
                Price = 10,
                Stock = 5
            };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_have_error_when_Price_is_zero()
        {
            var model = new ProductCreateDto
            {
                Name = "Valid Product Name",
                Description = "Valid description",
                Price = 0,
                Stock = 5
            };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Price);
        }

        [Fact]
        public void Should_have_error_when_Price_is_negative()
        {
            var model = new ProductCreateDto
            {
                Name = "Valid Product Name",
                Description = "Valid description",
                Price = -1,
                Stock = 5
            };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Price);
        }

        [Fact]
        public void Should_not_have_error_when_Price_is_positive()
        {
            var model = new ProductCreateDto
            {
                Name = "Valid Product Name",
                Description = "Valid description",
                Price = 10,
                Stock = 5
            };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Price);
        }

        [Fact]
        public void Should_have_error_when_Stock_is_negative()
        {
            var model = new ProductCreateDto
            {
                Name = "Valid Product Name",
                Description = "Valid description",
                Price = 10,
                Stock = -1
            };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Stock);
        }

        [Fact]
        public void Should_not_have_error_when_Stock_is_zero()
        {
            var model = new ProductCreateDto
            {
                Name = "Valid Product Name",
                Description = "Valid description",
                Price = 10,
                Stock = 0
            };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Stock);
        }

        [Fact]
        public void Should_not_have_error_when_Stock_is_positive()
        {
            var model = new ProductCreateDto
            {
                Name = "Valid Product Name",
                Description = "Valid description",
                Price = 10,
                Stock = 5
            };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Stock);
        }
    }

    public class ProductUpdateValidatorTests
    {
        private readonly ProductUpdateValidator _validator;

        public ProductUpdateValidatorTests()
        {
            _validator = new ProductUpdateValidator();
        }

        [Fact]
        public void Should_have_error_when_Name_is_too_long()
        {
            var model = new ProductUpdateDto
            {
                Name = new string('A', 201), // 201 символ
            };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_not_have_error_when_Name_is_valid()
        {
            var model = new ProductUpdateDto
            {
                Name = "Valid Product Name",
            };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_not_have_error_when_Name_is_null()
        {
            var model = new ProductUpdateDto
            {
                Name = null,
            };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_have_error_when_Description_is_too_long()
        {
            var model = new ProductUpdateDto
            {
                Description = new string('A', 1001), // 1001 символ
            };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_not_have_error_when_Description_is_valid()
        {
            var model = new ProductUpdateDto
            {
                Description = "Valid description",
            };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_not_have_error_when_Description_is_null()
        {
            var model = new ProductUpdateDto
            {
                Description = null,
            };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_have_error_when_Price_is_zero()
        {
            var model = new ProductUpdateDto
            {
                Price = 0,
            };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Price);
        }

        [Fact]
        public void Should_have_error_when_Price_is_negative()
        {
            var model = new ProductUpdateDto
            {
                Price = -1,
            };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Price);
        }

        [Fact]
        public void Should_not_have_error_when_Price_is_positive()
        {
            var model = new ProductUpdateDto
            {
                Price = 10,
            };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Price);
        }

        [Fact]
        public void Should_not_have_error_when_Price_is_null()
        {
            var model = new ProductUpdateDto
            {
                Price = null,
            };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Price);
        }

        [Fact]
        public void Should_have_error_when_Stock_is_negative()
        {
            var model = new ProductUpdateDto
            {
                Stock = -1,
            };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Stock);
        }

        [Fact]
        public void Should_not_have_error_when_Stock_is_zero()
        {
            var model = new ProductUpdateDto
            {
                Stock = 0,
            };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Stock);
        }

        [Fact]
        public void Should_not_have_error_when_Stock_is_positive()
        {
            var model = new ProductUpdateDto
            {
                Stock = 5,
            };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Stock);
        }

        [Fact]
        public void Should_not_have_error_when_Stock_is_null()
        {
            var model = new ProductUpdateDto
            {
                Stock = null,
            };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Stock);
        }
    }
}