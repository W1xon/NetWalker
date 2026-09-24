using NetWalker.Application.Common.Interfaces.Security;
using NetWalker.Application.Common.Models;
using NetWalker.Application.DTOs.Auth;
using NetWalker.Domain.Entities;
using NetWalker.Domain.Users;
using System.Security.Cryptography;
using NetWalker.Application.Common.Interfaces.Persistence;

namespace NetWalker.Application.Services.Auth;

public class UserSessionService : IUserSessionService
{
    private readonly IJwtProvider _jwtProvider;
    private readonly IUserSessionRepository _sessionRepository;
    private readonly IUserRepository _userRepository;
    private readonly TimeSpan _refreshTokenExpiration = TimeSpan.FromDays(7);

    public UserSessionService(IJwtProvider jwtProvider,
        IUserSessionRepository sessionRepository, 
        IUserRepository userRepository)
    {
        _jwtProvider = jwtProvider;
        _sessionRepository = sessionRepository;
        _userRepository = userRepository;
    }
    public async Task<Result<AuthResponse>> CreateSessionAsync(User user, CancellationToken cancellationToken = default)
    {
        var (token, refreshToken) = GenerateTokens(user);
        var refreshTokenHash = HashToken(refreshToken);
        await _sessionRepository.AddAsync(new UserSession(user.Id, refreshTokenHash, _refreshTokenExpiration), cancellationToken );
        return Result<AuthResponse>.Success(new AuthResponse(token, refreshToken));
    }

    public async Task<Result<AuthResponse>> RefreshSessionAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var incomingHash = HashToken(refreshToken);
        var session = await _sessionRepository.GetByTokenHashAsync(incomingHash, cancellationToken);
        if (session is null || !session.IsActive)
        {
            return  Result<AuthResponse>.Failure("Invalid refresh token.");
        }
        var user  = await _userRepository.GetByIdAsync(session.UserId, cancellationToken);
        if (user is null) return Result<AuthResponse>.Failure("User not found.");
        await RevokeSessionAsync(refreshToken, cancellationToken);
        var (newToken, newRefreshToken) = GenerateTokens(user);
        var refreshTokenHash = HashToken(newRefreshToken);
        await _sessionRepository.AddAsync(new UserSession(user.Id, refreshTokenHash, _refreshTokenExpiration), cancellationToken);
        return Result<AuthResponse>.Success(new AuthResponse(newToken, newRefreshToken));
    }

    public async Task<Result> RevokeSessionAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var incomingHash = HashToken(refreshToken);
        var session = await _sessionRepository.GetByTokenHashAsync(incomingHash, cancellationToken);
        if (session is not null )
        {
            await _sessionRepository.RemoveAsync(session, cancellationToken);
        }
        return Result.Success();
    }
    
    private (string, string) GenerateTokens(User user)
    {
        var token = _jwtProvider.GenerateToken(user);
        var refreshToken = GenerateRefreshToken();
        return (token, refreshToken);
    }
    private string GenerateRefreshToken(int byteSize = 64)
    {
        var randomBytes = RandomNumberGenerator.GetBytes(byteSize);
        return Convert.ToBase64String(randomBytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=');
    }
    private static string HashToken(string token)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(token);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }
}