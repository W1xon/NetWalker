using NetWalker.Application.Common.Interfaces;
using NetWalker.Application.Common.Interfaces.Persistence;
using NetWalker.Application.Common.Interfaces.Security;
using NetWalker.Application.Common.Models;
using NetWalker.Application.DTOs.Auth;
using NetWalker.Domain;
using NetWalker.Domain.Users;

namespace NetWalker.Application.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserRepository _userRepository;
    private readonly IUserSessionService _userSessionService;
    public AuthService(IPasswordHasher passwordHasher, 
        IUserRepository userRepository,
        IUserSessionService userSessionService)
    {
        _passwordHasher = passwordHasher;
        _userRepository = userRepository;
        _userSessionService = userSessionService;
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByNickAsync(request.Name, cancellationToken);
        if (user is null)
        {
            return Result<AuthResponse>.Failure("Неверный логин или пароль");
        }

        bool isPasswordValid = _passwordHasher.Verify(request.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            return Result<AuthResponse>.Failure("Неверный логин или пароль");
        }

        return await _userSessionService.CreateSessionAsync(user);
    }
    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var existingUser = await _userRepository.GetByNickAsync(request.Name, cancellationToken);
        if (existingUser is not null)
            return Result<AuthResponse>.Failure("Пользователь с таким ником уже существует");

        var passwordHash = _passwordHasher.Create(request.Password);
        var user = new User(request.Name, passwordHash);

        try
        {
            await _userRepository.AddAsync(user, cancellationToken);
        }
        catch
        {
            return Result<AuthResponse>.Failure("Пользователь с таким ником уже существует");
        }

        return await _userSessionService.CreateSessionAsync(user, cancellationToken);
    }

    public async Task<Result<AuthResponse>> ChangePassword(ChangePasswordRequest request, Guid id, CancellationToken cancellationToken = default)
    {
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
        try
        {
            await _userRepository.UpdateAsync(user, cancellationToken);
        }
        catch
        {
            return Result<AuthResponse>.Failure("Ошибка обновления пароля");
        }
        
        return await _userSessionService.CreateSessionAsync(user, cancellationToken);
    }
}