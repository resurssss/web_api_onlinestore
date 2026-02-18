using FluentValidation;
using OnlineStore.Core.DTOs;

namespace OnlineStore.Core.Validators
{
    public class CartItemUpdateValidator : AbstractValidator<CartItemUpdateDto>
    {
        public CartItemUpdateValidator()
        {
            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Количество должно быть больше 0");
        }
    }
}