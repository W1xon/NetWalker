using FluentValidation;
using NetWalker.Application.Common.Interfaces;
using NetWalker.Application.Common.Interfaces.Persistence;
using NetWalker.Application.Common.Interfaces.Security;
using NetWalker.Application.Common.Models;
using NetWalker.Application.Common.Validators;
using NetWalker.Application.DTOs.Auth;
using NetWalker.Domain;

namespace NetWalker.Application.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserRepository _userRepository;
    private readonly IJwtProvider _jwtProvider;
    private readonly IValidator<ChangePasswordRequest> _changePasswordValidator;
    private readonly IValidator<LoginRequest> _loginValidator;
    private readonly IValidator<RegisterRequest> _registerValidator;

    public AuthService(IPasswordHasher passwordHasher, 
        IUserRepository userRepository, 
        IJwtProvider jwtProvider,
        IValidator<ChangePasswordRequest> changePasswordValidator,
        IValidator<LoginRequest> loginValidator,
        IValidator<RegisterRequest> registerValidator)
    {
        _passwordHasher = passwordHasher;
        _userRepository = userRepository;
        _jwtProvider = jwtProvider;
        _changePasswordValidator = changePasswordValidator;
        _loginValidator = loginValidator;
        _registerValidator = registerValidator;
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _loginValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            string firstError = validationResult.Errors.First().ErrorMessage;
            return Result<AuthResponse>.Failure(firstError);
        }
        
        var user = await _userRepository.GetByIdAsync(request.Name, cancellationToken);
        if (user is null)
        {
            return Result<AuthResponse>.Failure("Неверный логин или пароль");
        }

        bool isPasswordValid = _passwordHasher.Verify(request.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            return Result<AuthResponse>.Failure("Неверный логин или пароль");
        }

        var token = _jwtProvider.GenerateToken(user);

        return Result<AuthResponse>.Success( new AuthResponse(token));
    }
    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _registerValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            string firstError = validationResult.Errors.First().ErrorMessage;
            return Result<AuthResponse>.Failure(firstError);
        }
        var existingUser = await _userRepository.GetByIdAsync(request.Name, cancellationToken);
        if (existingUser is not null)
            return Result<AuthResponse>.Failure("Пользователь с таким ником уже существует");

        var passwordHash = _passwordHasher.Create(request.Password);
        var user = new User(request.Name, passwordHash);

        var addResult = await _userRepository.AddAsync(user, cancellationToken);
        if(!addResult.IsSuccess)
            return Result<AuthResponse>.Failure("Пользователь с таким ником уже существует");

        var token = _jwtProvider.GenerateToken(user);
        return Result<AuthResponse>.Success(new AuthResponse(token));
    }

    public async Task<Result<AuthResponse>> ChangePassword(ChangePasswordRequest request, Guid id, CancellationToken cancellationToken = default)
    {
        var validationResult = await _changePasswordValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            string firstError = validationResult.Errors.First().ErrorMessage;
            return Result<AuthResponse>.Failure(firstError);
        }
        
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        
        if(user is null)
            return Result<AuthResponse>.Failure("Неверный логин или пароль");

        bool isValidPassword = _passwordHasher.Verify(request.OldPassword, user.PasswordHash);
        
        if(!isValidPassword)
            return Result<AuthResponse>.Failure("Неверный логин или пароль");

        var passwordHash = _passwordHasher.Create(request.NewPassword);

        var succes = user.TryChangePassword(passwordHash);
        if(!succes)
            return Result<AuthResponse>.Failure("Пароли совпадают");
        var updateResult = await _userRepository.UpdateAsync(user, cancellationToken);
        if(!updateResult.IsSuccess)
            return Result<AuthResponse>.Failure("Ошибка обновления пароля");

        var token = _jwtProvider.GenerateToken(user);
        
        return Result<AuthResponse>.Success(new AuthResponse(token));
    }
}