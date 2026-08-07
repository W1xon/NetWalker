using NetWalker.Application.Common.Interfaces;
using NetWalker.Application.Common.Interfaces.Persistence;
using NetWalker.Application.Common.Interfaces.Security;
using NetWalker.Application.Common.Models;
using NetWalker.Application.DTOs.Auth;
using NetWalker.Domain;

namespace NetWalker.Application.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserRepository _userRepository;
    private readonly IJwtProvider _jwtProvider;

    public AuthService(IPasswordHasher passwordHasher, IUserRepository userRepository, IJwtProvider jwtProvider)
    {
        _passwordHasher = passwordHasher;
        _userRepository = userRepository;
        _jwtProvider = jwtProvider;
    }

    public async Task<Result<AuthResult>> LoginAsync(string name, string password, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByNickAsync(name, cancellationToken);
        if (user is null)
        {
            return Result<AuthResult>.Failure("Неверный логин или пароль");
        }

        var isPasswordValid = _passwordHasher.Verify(password, user.PasswordHash);
        if (!isPasswordValid)
        {
            return Result<AuthResult>.Failure("Неверный логин или пароль");
        }

        var token = _jwtProvider.GenerateToken(user);

        return Result<AuthResult>.Success( new AuthResult(token));
    }
    public async Task<Result<AuthResult>> RegisterAsync(string name, string password, CancellationToken cancellationToken = default)
    {
        var existingUser = await _userRepository.GetByNickAsync(name, cancellationToken);
        if (existingUser is not null)
            return Result<AuthResult>.Failure("Пользователь с таким ником уже существует");

        var passwordHash = _passwordHasher.Create(password);
        var user = new User(name, passwordHash);

        var addResult = await _userRepository.AddAsync(user, cancellationToken);
        if(!addResult.IsSuccess)
            return Result<AuthResult>.Failure("Пользователь с таким ником уже существует");

        var token = _jwtProvider.GenerateToken(user);
        return Result<AuthResult>.Success(new AuthResult(token));
    }
}