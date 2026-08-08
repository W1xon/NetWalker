using NetWalker.Application.Common.Models;
using NetWalker.Domain;

namespace NetWalker.Application.Common.Interfaces.Persistence;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(string name, CancellationToken token = default);
    Task<User?> GetByIdAsync(Guid id, CancellationToken token = default);
    
    Task<User?> GetByNickWithStatsAsync(string name, CancellationToken token = default);
    Task<User?> GetByIdWithStatsAsync(Guid id, CancellationToken token = default);
    
    Task<Result> UpdateAsync(User user, CancellationToken token = default);
    
    Task<Result> AddAsync(User user, CancellationToken token = default);
}