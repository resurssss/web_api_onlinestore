using FluentValidation;
using OnlineStore.Core.DTOs;

namespace OnlineStore.Core.Validators
{
    public class ResetPasswordDTOValidator : AbstractValidator<ResetPasswordDTO>
    {
        public ResetPasswordDTOValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("Токен обязателен для заполнения");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("Новый пароль обязателен для заполнения")
                .MinimumLength(8).WithMessage("Пароль должен содержать минимум 8 символов");

            RuleFor(x => x.ConfirmNewPassword)
                .Equal(x => x.NewPassword).WithMessage("Пароли не совпадают");
        }
    }
}