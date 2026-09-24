using NetWalker.Domain.Entities;

namespace NetWalker.Application.Common.Interfaces.Persistence;

public interface IUserSessionRepository
{
    Task AddAsync(UserSession session, CancellationToken cancellationToken = default);
    Task RemoveAsync(UserSession session, CancellationToken cancellationToken = default);
    Task UpdateAsync (UserSession session, CancellationToken cancellationToken = default);
    Task<UserSession?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);
}