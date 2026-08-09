using FluentValidation;

namespace NetWalker.Application.Common.Validators.Extensions;

public static class PasswordValidationExtensions
{
    public static IRuleBuilderOptions<T, string> ValidPassword<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
        .NotEmpty().WithMessage("Пароль обязателен")
        .MinimumLength(8).WithMessage("Пароль должен быть минимум 8 символов")
        .Matches("[A-Z]").WithMessage("Пароль должен содержать хотя бы одну заглавную букву")
        .Matches("[0-9]").WithMessage("Пароль должен содержать хотя бы одну цифру");
    }
}