using Microsoft.AspNetCore.SignalR;
using NetWalker.Application.DTOs.Room;
using NetWalker.Application.Services;

namespace NetWalker.Infrastructure.RoomHubs;

public class RoomHubNotifier : IRoomHubNotifier
{
    private readonly IHubContext<RoomSessionHub> _hubContext;
    
    public RoomHubNotifier(IHubContext<RoomSessionHub> hubContext)
    {
        _hubContext = hubContext;
    }
    public async Task NotifyMessageReceivedAsync(string roomId, string message, CancellationToken token = default)
    {
        await _hubContext.Clients.Group(roomId).SendAsync("ReceiveMessage", message, token);
    }

    public async Task NotifyCallerAsync(string connectionId, string message, CancellationToken token = default)
    {
        await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveError", message, token);
    }

    public async Task NotifyJoinedRoomAsync(string roomId, RoomPlayerDto playerDto, CancellationToken token = default)
    {
        await _hubContext.Clients.Group(roomId).SendAsync("PlayerJoined", playerDto, token);
    }

    public async Task NotifyLeavedRoomAsync(string roomId, string name, CancellationToken token = default)
    {
        await _hubContext.Clients.Group(roomId).SendAsync("PlayerLeaved", name, token);
    }
}