using FluentValidation.TestHelper;
using OnlineStore.Core.Validators;
using OnlineStore.Core.DTOs;
using Xunit;

namespace OnlineStore.Tests.Validators
{
    public class CouponCreateValidatorTests
    {
        private readonly CouponCreateValidator _validator;

        public CouponCreateValidatorTests()
        {
            _validator = new CouponCreateValidator();
        }

        [Fact]
        public void Should_have_error_when_Code_is_empty()
        {
            var model = new CouponCreateDto 
            { 
                Code = "", 
                DiscountPercent = 10,
                ExpirationDate = DateTime.UtcNow.AddDays(30)
            };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Code);
        }

        [Fact]
        public void Should_have_error_when_Code_is_too_long()
        {
            var model = new CouponCreateDto
            {
                Code = new string('A', 51), // 51 символ
                DiscountPercent = 10,
                ExpirationDate = DateTime.UtcNow.AddDays(30)
            };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Code);
        }

        [Fact]
        public void Should_not_have_error_when_Code_is_valid()
        {
            var model = new CouponCreateDto
            {
                Code = "VALID123",
                DiscountPercent = 10,
                ExpirationDate = DateTime.UtcNow.AddDays(30)
            };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Code);
        }

        [Fact]
        public void Should_have_error_when_DiscountPercent_is_less_than_0_1()
        {
            var model = new CouponCreateDto
            {
                Code = "VALID123",
                DiscountPercent = 0.05m, // Меньше 0.1
                ExpirationDate = DateTime.UtcNow.AddDays(30)
            };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.DiscountPercent);
        }

        [Fact]
        public void Should_have_error_when_DiscountPercent_is_greater_than_100()
        {
            var model = new CouponCreateDto
            {
                Code = "VALID123",
                DiscountPercent = 101,
                ExpirationDate = DateTime.UtcNow.AddDays(30)
            };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.DiscountPercent);
        }

        [Fact]
        public void Should_not_have_error_when_DiscountPercent_is_valid()
        {
            var model = new CouponCreateDto
            {
                Code = "VALID123",
                DiscountPercent = 10,
                ExpirationDate = DateTime.UtcNow.AddDays(30)
            };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.DiscountPercent);
        }

        [Fact]
        public void Should_have_error_when_ExpirationDate_is_in_past()
        {
            var model = new CouponCreateDto
            {
                Code = "VALID123",
                DiscountPercent = 10,
                ExpirationDate = DateTime.UtcNow.AddDays(-1) // В прошлом
            };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.ExpirationDate);
        }

        [Fact]
        public void Should_not_have_error_when_ExpirationDate_is_in_future()
        {
            var model = new CouponCreateDto
            {
                Code = "VALID123",
                DiscountPercent = 10,
                ExpirationDate = DateTime.UtcNow.AddDays(30)
            };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.ExpirationDate);
        }

        [Fact]
        public void Should_have_error_when_UsageLimit_is_zero()
        {
            var model = new CouponCreateDto
            {
                Code = "VALID123",
                DiscountPercent = 10,
                ExpirationDate = DateTime.UtcNow.AddDays(30),
                UsageLimit = 0
            };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.UsageLimit);
        }

        [Fact]
        public void Should_have_error_when_UsageLimit_is_negative()
        {
            var model = new CouponCreateDto
            {
                Code = "VALID123",
                DiscountPercent = 10,
                ExpirationDate = DateTime.UtcNow.AddDays(30),
                UsageLimit = -1
            };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.UsageLimit);
        }

        [Fact]
        public void Should_not_have_error_when_UsageLimit_is_positive()
        {
            var model = new CouponCreateDto
            {
                Code = "VALID123",
                DiscountPercent = 10,
                ExpirationDate = DateTime.UtcNow.AddDays(30),
                UsageLimit = 5
            };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.UsageLimit);
        }

        [Fact]
        public void Should_not_have_error_when_UsageLimit_is_null()
        {
            var model = new CouponCreateDto
            {
                Code = "VALID123",
                DiscountPercent = 10,
                ExpirationDate = DateTime.UtcNow.AddDays(30),
                UsageLimit = null
            };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.UsageLimit);
        }
    }

    public class CouponUpdateValidatorTests
    {
        private readonly CouponUpdateValidator _validator;

        public CouponUpdateValidatorTests()
        {
            _validator = new CouponUpdateValidator();
        }

        [Fact]
        public void Should_have_error_when_Code_is_too_long()
        {
            var model = new CouponUpdateDto
            {
                Code = new string('A', 51), // 51 символ
            };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Code);
        }

        [Fact]
        public void Should_not_have_error_when_Code_is_valid()
        {
            var model = new CouponUpdateDto
            {
                Code = "VALID123",
            };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Code);
        }

        [Fact]
        public void Should_not_have_error_when_Code_is_null()
        {
            var model = new CouponUpdateDto
            {
                Code = null,
            };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Code);
        }

        [Fact]
        public void Should_have_error_when_DiscountPercent_is_less_than_0_1()
        {
            var model = new CouponUpdateDto
            {
                DiscountPercent = 0.05m, // Меньше 0.1
            };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.DiscountPercent);
        }

        [Fact]
        public void Should_have_error_when_DiscountPercent_is_greater_than_100()
        {
            var model = new CouponUpdateDto
            {
                DiscountPercent = 101,
            };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.DiscountPercent);
        }

        [Fact]
        public void Should_not_have_error_when_DiscountPercent_is_valid()
        {
            var model = new CouponUpdateDto
            {
                DiscountPercent = 10,
            };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.DiscountPercent);
        }

        [Fact]
        public void Should_not_have_error_when_DiscountPercent_is_null()
        {
            var model = new CouponUpdateDto
            {
                DiscountPercent = null,
            };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.DiscountPercent);
        }

        [Fact]
        public void Should_have_error_when_ExpirationDate_is_in_past()
        {
            var model = new CouponUpdateDto
            {
                ExpirationDate = DateTime.UtcNow.AddDays(-1) // В прошлом
            };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.ExpirationDate);
        }

        [Fact]
        public void Should_not_have_error_when_ExpirationDate_is_in_future()
        {
            var model = new CouponUpdateDto
            {
                ExpirationDate = DateTime.UtcNow.AddDays(30)
            };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.ExpirationDate);
        }

        [Fact]
        public void Should_not_have_error_when_ExpirationDate_is_null()
        {
            var model = new CouponUpdateDto
            {
                ExpirationDate = null
            };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.ExpirationDate);
        }

        [Fact]
        public void Should_have_error_when_UsageLimit_is_zero()
        {
            var model = new CouponUpdateDto
            {
                UsageLimit = 0
            };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.UsageLimit);
        }

        [Fact]
        public void Should_have_error_when_UsageLimit_is_negative()
        {
            var model = new CouponUpdateDto
            {
                UsageLimit = -1
            };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.UsageLimit);
        }

        [Fact]
        public void Should_not_have_error_when_UsageLimit_is_positive()
        {
            var model = new CouponUpdateDto
            {
                UsageLimit = 5
            };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.UsageLimit);
        }

        [Fact]
        public void Should_not_have_error_when_UsageLimit_is_null()
        {
            var model = new CouponUpdateDto
            {
                UsageLimit = null
            };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.UsageLimit);
        }
    }
}