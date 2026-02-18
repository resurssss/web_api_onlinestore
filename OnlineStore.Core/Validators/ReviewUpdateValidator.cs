using FluentValidation;
using OnlineStore.Core.DTOs;

namespace OnlineStore.Core.Validators
{
    public class ReviewUpdateValidator : AbstractValidator<ReviewUpdateDto>
    {
        public ReviewUpdateValidator()
        {
            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5).WithMessage("Рейтинг должен быть от 1 до 5");

            RuleFor(x => x.Comment)
                .MaximumLength(1000).WithMessage("Комментарий не может превышать 1000 символов");
        }
    }
}