using FluentValidation;
using OnlineStore.Core.DTOs;

namespace OnlineStore.Core.Validators
{
    public class ReviewValidator : AbstractValidator<ReviewCreateDto>
    {
        public ReviewValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("ProductId обязателен");

            RuleFor(x => x.Author)
                .NotEmpty().WithMessage("Автор обязателен")
                .MaximumLength(100).WithMessage("Имя автора не может превышать 100 символов");

            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5).WithMessage("Рейтинг должен быть от 1 до 5");

            RuleFor(x => x.Comment)
                .MaximumLength(1000).WithMessage("Комментарий не может превышать 1000 символов");
        }
    }
}