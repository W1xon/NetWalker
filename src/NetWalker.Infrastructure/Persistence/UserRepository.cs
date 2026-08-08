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
    public async Task<User?> GetByIdAsync(string name, CancellationToken token = default)
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

    public async Task<Result> UpdateAsync(User user, CancellationToken token = default)
    {
        try
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync(token);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Ошибка обновления пользователя: {ex.Message}");
        }
    }

    public async Task<Result> AddAsync(User user, CancellationToken token = default)
    {
        try
        { 
            _context.Users.Add(user);
            await _context.SaveChangesAsync(token);
            return Result.Success();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException
                                           {
                                               SqlState: PostgresErrorCodes.UniqueViolation
                                           })
        {
            return Result.Failure("Пользователь с таким ником уже существует");
        }
        catch (Exception ex)
        {
            return Result.Failure($"Ошибка сохранения пользователя: {ex.Message}");
        }
    }
}