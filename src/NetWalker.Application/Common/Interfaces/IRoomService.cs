using NetWalker.Application.Common.Models;
using NetWalker.Application.DTOs.Room;

namespace NetWalker.Application.Common.Interfaces;

public interface IRoomService
{
    Task<Result<CreateRoomResponse>> CreateRoomAsync(Guid hostId, CreateRoomRequest request, CancellationToken token = default);
    
    Task<Result<GetRoomResponse>> GetRoomByCodeAsync(string sessionCode, CancellationToken token = default);
    Task<IReadOnlyList<GetRoomResponse>> GetActiveRoomsAsync(CancellationToken token = default);

    Task<Result> JoinRoomAsync(Guid playerId, string sessionCode, CancellationToken token = default);
    Task<Result> LeaveRoomAsync(Guid playerId, string sessionCode, CancellationToken token = default);
}