using NetWalker.Domain.Users;

namespace NetWalker.Application.Common.Interfaces.Persistence;

public interface IUserSessionRepository
{
    Task AddAsync(UserSession session, CancellationToken cancellationToken = default);
    Task RemoveAsync(UserSession session, CancellationToken cancellationToken = default);
    Task UpdateAsync (UserSession session, CancellationToken cancellationToken = default);
    Task<UserSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}