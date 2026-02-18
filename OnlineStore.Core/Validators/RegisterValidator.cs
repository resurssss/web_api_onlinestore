using FluentValidation;
using OnlineStore.Core.DTOs;

namespace OnlineStore.Core.Validators
{
    public class RegisterValidator : AbstractValidator<RegisterDTO>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email обязателен")
                .EmailAddress().WithMessage("Некорректный формат email")
                .MaximumLength(200).WithMessage("Email слишком длинный");

            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Имя пользователя обязательно")
                .Length(3, 50).WithMessage("Имя пользователя должно быть от 3 до 50 символов")
                .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("Имя пользователя может содержать только буквы, цифры и подчеркивания");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Пароль обязателен")
                .MinimumLength(8).WithMessage("Пароль должен содержать минимум 8 символов")
                .Matches(@"[A-Z]").WithMessage("Пароль должен содержать хотя бы одну заглавную букву")
                .Matches(@"[a-z]").WithMessage("Пароль должен содержать хотя бы одну строчную букву")
                .Matches(@"[0-9]").WithMessage("Пароль должен содержать хотя бы одну цифру")
                .Matches(@"[!@#$%^&*(),.?""':{}|<>]").WithMessage("Пароль должен содержать хотя бы один специальный символ");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithMessage("Пароли не совпадают");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Имя обязательно")
                .MaximumLength(50).WithMessage("Имя слишком длинное");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Фамилия обязательна")
                .MaximumLength(50).WithMessage("Фамилия слишком длинная");

            RuleFor(x => x.DateOfBirth)
                .LessThan(DateTime.Today).WithMessage("Дата рождения не может быть в будущем");

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Некорректный формат номера телефона").When(x => !string.IsNullOrEmpty(x.PhoneNumber));
        }
    }
}