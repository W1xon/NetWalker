using NetWalker.Application.Common.Models;
using NetWalker.Application.DTOs.Auth;

namespace NetWalker.Application.Common.Interfaces;

public interface IAuthService
{
    Task<Result<AuthResult>> LoginAsync(string name, string password, CancellationToken cancellationToken = default);
    Task<Result<AuthResult>> RegisterAsync(string name, string password, CancellationToken cancellationToken = default);
}