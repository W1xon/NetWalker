using Microsoft.EntityFrameworkCore;
using NetWalker.Application.Common.Interfaces.Persistence;
using NetWalker.Domain.Rooms;

namespace NetWalker.Infrastructure.Persistence;

public class RoomRepository : IRoomRepository
{
    private readonly AppDbContext _context;

    public RoomRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Rooms
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<Room?> GetBySessionCodeAsync(string sessionCode, CancellationToken cancellationToken = default)
    {
        return await _context.Rooms
            .FirstOrDefaultAsync(r => r.SessionCode == sessionCode, cancellationToken);
    }

    public async Task<IReadOnlyList<Room>> GetActiveRoomsAsync(CancellationToken cancellationToken = default)
    { 
        return await _context.Rooms
            .AsNoTracking()
            .Where(r => r.Status == RoomStatus.Lobby)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Room room, CancellationToken cancellationToken = default)
    {
        await _context.Rooms.AddAsync(room, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Room room, CancellationToken cancellationToken = default)
    {
        _context.Rooms.Update(room);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public void Remove(Room room)
    {
        _context.Rooms.Remove(room);
    }
}