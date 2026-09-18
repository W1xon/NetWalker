namespace NetWalker.Application.Common.Interfaces.Persistence;

using NetWalker.Domain.Rooms;

public interface IRoomRepository
{
    Task<Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Room?> GetBySessionCodeAsync(string sessionCode, CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<Room>> GetActiveRoomsAsync(CancellationToken cancellationToken = default);

    Task AddAsync(Room room, CancellationToken cancellationToken = default);
    Task UpdateAsync(Room room, CancellationToken cancellationToken = default); 
    Task Remove(Room room, CancellationToken cancellationToken = default);
    Task Remove(string sessionCode, CancellationToken cancellationToken = default);
}