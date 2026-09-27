using System.Buffers.Text;
using NetWalker.Application.Common.Interfaces.Security;
using NetWalker.Application.Common.Models;
using NetWalker.Application.DTOs.Auth;
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

    public async Task<Result<AuthResponse>> CreateSessionAsync(User user, SessionContextDto sessionContext, CancellationToken cancellationToken = default)
    {
        var rawSecret = GenerateRandomSecret();
        var refreshTokenHash = HashToken(rawSecret);
        
        var session = new UserSession(user.Id,
            refreshTokenHash,
            sessionContext.DeviceType,
            sessionContext.Os,
            sessionContext.IpAddress,
            _refreshTokenExpiration);
        
        await _sessionRepository.AddAsync(session, cancellationToken );
        var refreshToken = $"{session.Id}.{rawSecret}";
        var accessToken = _jwtProvider.GenerateToken(user);
        return Result<AuthResponse>.Success(new AuthResponse(accessToken, refreshToken));
    }

    public async Task<Result<AuthResponse>> RefreshSessionAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        if (!IsValidTokenFormat(refreshToken, out var sessionId, out var incomingSecret))
            return Result.Failure<AuthResponse>("Invalid refresh token format");
        
        var session = await _sessionRepository.GetByIdAsync(sessionId, cancellationToken);
        if (session is null || !session.IsActive)
            return  Result<AuthResponse>.Failure("Session not found or revoked");

        var incomingHash = HashToken(incomingSecret);
        var validationResult = session.CheckToken(incomingHash);
        
        if (validationResult == TokenValidationResult.Invalid)
            return await RevokeInvalidSession(session);
        
        var user  = await _userRepository.GetByIdAsync(session.UserId, cancellationToken);
        if (user is null) return Result<AuthResponse>.Failure("User not found.");

        var newSecret = GenerateRandomSecret();
        var tokenHash = HashToken(newSecret);

        if (validationResult == TokenValidationResult.ValidCurrent)
            session.RotateToken(tokenHash);

        if (validationResult == TokenValidationResult.ValidGracePeriod)
            session.TokenHash = tokenHash;
        
        session.ExpiresAt = DateTime.UtcNow.Add(_refreshTokenExpiration);
        await _sessionRepository.UpdateAsync(session, cancellationToken);

        var newRefreshToken = $"{session.Id}.{newSecret}";
        var newAccessToken = _jwtProvider.GenerateToken(user);
        return Result<AuthResponse>.Success(new AuthResponse(newAccessToken, newRefreshToken));
    }

    public async Task<Result> RevokeSessionAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var parts = refreshToken.Split(".");
        if (parts.Length == 2 && Guid.TryParse(parts[0], out var sessionId))
        {
            var session = await _sessionRepository.GetByIdAsync(sessionId, cancellationToken);
            if (session is not null )
            {
                session.Revoke();
                await _sessionRepository.UpdateAsync(session, cancellationToken);
            }
        }
        return Result.Success();
    }
    
    private string GenerateRandomSecret(int byteSize = 64)
    {
        var randomBytes = RandomNumberGenerator.GetBytes(byteSize);
        return Base64Url.EncodeToString(randomBytes);
    }
    private string HashToken(string token)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(token);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }

    private bool IsValidTokenFormat(string token, out Guid sessionId, out string secret)
    {
        sessionId = Guid.Empty;
        secret = string.Empty;

        var parts = token.Split('.');
        if (parts.Length != 2 || !Guid.TryParse(parts[0], out sessionId))
            return false;

        secret = parts[1];
        return true;
    }
    private async Task<Result<AuthResponse>> RevokeInvalidSession(UserSession session)
    {
        session.Revoke();
        await _sessionRepository.UpdateAsync(session);
        return Result<AuthResponse>.Failure("Session not found or revoked");
    }
}