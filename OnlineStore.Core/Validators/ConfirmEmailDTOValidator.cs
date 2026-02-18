using FluentValidation;
using OnlineStore.Core.DTOs;

namespace OnlineStore.Core.Validators
{
    public class ConfirmEmailDTOValidator : AbstractValidator<ConfirmEmailDTO>
    {
        public ConfirmEmailDTOValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId обязателен для заполнения");

            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("Токен обязателен для заполнения");
        }
    }
}