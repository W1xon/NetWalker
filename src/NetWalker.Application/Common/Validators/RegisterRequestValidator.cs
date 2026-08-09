using FluentValidation;
using NetWalker.Application.Common.Validators.Extensions;
using NetWalker.Application.DTOs.Auth;

namespace NetWalker.Application.Common.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty().WithMessage("Имя не может быть пустым")
            .Length(3, 32).WithMessage("Длина имени должна быть от 3 до 32 символов")
            .Matches(@"^[a-zA-Z0-9_-]+$").WithMessage("Имя может содержать только латиницу, цифры, _ и -");

        RuleFor(x => x.Password)
            .ValidPassword(); 
    }
}