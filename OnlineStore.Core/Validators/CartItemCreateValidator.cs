using FluentValidation;
using OnlineStore.Core.DTOs;

namespace OnlineStore.Core.Validators
{
    public class CartItemCreateValidator : AbstractValidator<CartItemCreateDto>
    {
        public CartItemCreateValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("ProductId обязателен");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Количество должно быть больше 0");
        }
    }
}