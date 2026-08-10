namespace NetWalker.Application.Common.Interfaces.Persistence;

using NetWalker.Domain;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken token = default);
    Task<User?> GetByNickAsync(string nick, CancellationToken token = default);
    
    Task<User?> GetByIdWithStatsAsync(Guid id, CancellationToken token = default);
    Task<User?> GetByNickWithStatsAsync(string nick, CancellationToken token = default);
    
    Task AddAsync(User user, CancellationToken token = default);
    Task UpdateAsync(User user, CancellationToken token);
    void Remove(User user);
}