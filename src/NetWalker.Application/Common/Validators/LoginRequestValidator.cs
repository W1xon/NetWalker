using FluentValidation;
using NetWalker.Application.DTOs.Auth;

namespace NetWalker.Application.Common.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Имя обязательно");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пароль обязателен");
    }
}