using NetWalker.Application.Common.Models;
using NetWalker.Application.DTOs.Auth;

namespace NetWalker.Application.Common.Interfaces;

public interface IAuthService
{
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request, SessionContextDto sessionContext, CancellationToken cancellationToken = default);
    Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, SessionContextDto sessionContext, CancellationToken cancellationToken = default);
    Task<Result<AuthResponse>> ChangePassword(ChangePasswordRequest request, Guid id, SessionContextDto sessionContext, CancellationToken cancellationToken = default);
}