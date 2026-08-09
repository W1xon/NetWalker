using FluentValidation;
using NetWalker.Application.Common.Validators.Extensions;
using NetWalker.Application.DTOs.Auth;

namespace NetWalker.Application.Common.Validators;

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.OldPassword)
            .NotEmpty().WithMessage("Старый пароль обязателен");

        RuleFor(x => x.NewPassword)
            .ValidPassword()
            .Must((req, newPass) => newPass != req.OldPassword)
            .WithMessage("Новый пароль не должен совпадать со старым");
    }
}