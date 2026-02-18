using FluentValidation;
using OnlineStore.Core.DTOs;

namespace OnlineStore.Core.Validators
{
    public class FavoriteCreateValidator : AbstractValidator<FavoriteCreateDto>
    {
        public FavoriteCreateValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("ProductId обязателен");
        }
    }
}