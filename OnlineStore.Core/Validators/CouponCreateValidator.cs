using FluentValidation;
using OnlineStore.Core.DTOs;

namespace OnlineStore.Core.Validators
{
    public class CouponCreateValidator : AbstractValidator<CouponCreateDto>
    {
        public CouponCreateValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Код купона обязателен")
                .MaximumLength(50).WithMessage("Код купона не может превышать 50 символов");

            RuleFor(x => x.DiscountPercent)
                .GreaterThanOrEqualTo(0.1m).WithMessage("Процент скидки должен быть не менее 0.1")
                .InclusiveBetween(0, 100).WithMessage("Процент скидки должен быть от 0 до 100");

            RuleFor(x => x.ExpirationDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("Дата окончания должна быть в будущем");

            RuleFor(x => x.UsageLimit)
                .GreaterThan(0).WithMessage("Лимит использования должен быть положительным числом");
        }
    }
}