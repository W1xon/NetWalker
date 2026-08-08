using NetWalker.Application.Common.Models;
using NetWalker.Application.DTOs.Auth;

namespace NetWalker.Application.Common.Interfaces;

public interface IAuthService
{
    Task<Result<AuthResponse>> LoginAsync(string name, string password, CancellationToken cancellationToken = default);
    Task<Result<AuthResponse>> RegisterAsync(string name, string password, CancellationToken cancellationToken = default);
}