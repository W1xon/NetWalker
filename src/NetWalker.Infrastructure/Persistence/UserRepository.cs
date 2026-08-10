using Microsoft.EntityFrameworkCore;
using NetWalker.Application.Common.Interfaces.Persistence;
using NetWalker.Application.Common.Models;
using NetWalker.Domain;
using Npgsql;

namespace NetWalker.Infrastructure.Persistence;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<User?> GetByNickAsync(string name, CancellationToken token = default)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Nick == name, token);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, token);
    }

    public async Task<User?> GetByNickWithStatsAsync(string name, CancellationToken token = default)
    {
        return await _context.Users
            .AsNoTracking()
            .Include(u => u.Stats)
            .FirstOrDefaultAsync(u => u.Nick == name, token);
    }

    public async Task<User?> GetByIdWithStatsAsync(Guid id, CancellationToken token = default)
    {
        return await _context.Users
            .AsNoTracking()
            .Include(u => u.Stats)
            .FirstOrDefaultAsync(u => u.Id == id, token);
    }

    public async Task UpdateAsync(User user, CancellationToken token)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync(token);
    }

    public void Remove(User user)
    {
        throw new NotImplementedException();
    }

    public async Task AddAsync(User user, CancellationToken token = default)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync(token);
    }
}