using FluentValidation;
using OnlineStore.Core.DTOs;
using OnlineStore.Core.Interfaces;

namespace OnlineStore.Core.Validators
{
    public class FavoriteValidator : AbstractValidator<FavoriteCreateDto>
    {
        public FavoriteValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId обязателен");

            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("ProductId обязателен");
        }
    }
}