using NetWalker.Application.Common.Models;
using NetWalker.Domain;

namespace NetWalker.Application.Common.Interfaces.Persistence;

public interface IUserRepository
{
    Task<User?> GetByNickAsync(string name, CancellationToken cancellationToken = default);
    
    Task<Result> AddAsync(User user, CancellationToken cancellationToken = default);
}