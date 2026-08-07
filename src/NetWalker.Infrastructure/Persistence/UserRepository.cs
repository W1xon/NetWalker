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
    public async Task<User?> GetByNickAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Nick == name, cancellationToken);
    }

    public async Task<Result> AddAsync(User user, CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.Users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
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