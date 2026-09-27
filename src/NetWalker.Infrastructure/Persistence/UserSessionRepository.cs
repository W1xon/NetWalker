using Microsoft.EntityFrameworkCore;
using NetWalker.Application.Common.Interfaces.Persistence;
using NetWalker.Domain.Users;

namespace NetWalker.Infrastructure.Persistence;

public class UserSessionRepository : IUserSessionRepository
{
    private readonly AppDbContext _context;
    public UserSessionRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task AddAsync(UserSession session, CancellationToken cancellationToken = default)
    {
        _context.UserSessions.Add(session);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAsync(UserSession session, CancellationToken cancellationToken = default)
    {
        _context.UserSessions.Remove(session);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(UserSession session, CancellationToken cancellationToken = default)
    {
        _context.UserSessions.Update(session);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<UserSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    { 
        return await _context.UserSessions.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }
}