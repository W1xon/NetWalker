using NetWalker.Application.DTOs.Room;

namespace NetWalker.Application.Services;

public interface IRoomHubNotifier
{
    Task NotifyMessageReceivedAsync(string roomId, string message, CancellationToken token = default);
    Task NotifyCallerAsync(string connectionId, string message, CancellationToken token = default);
    Task NotifyJoinedRoomAsync(string roomId, RoomPlayerDto playerDto, CancellationToken token = default);
    Task NotifyLeavedRoomAsync(string roomId, string name, CancellationToken token = default);
}