using NetWalker.Application.Common.Models;
using NetWalker.Application.DTOs.Auth;
using NetWalker.Domain.Users;

namespace NetWalker.Application.Common.Interfaces.Security;

public interface IUserSessionService 
{
    Task<Result<AuthResponse>> CreateSessionAsync(User user, SessionContextDto sessionContext, CancellationToken cancellationToken = default);
    Task<Result<AuthResponse>> RefreshSessionAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<Result> RevokeSessionAsync(string refreshToken, CancellationToken cancellationToken = default);
} 