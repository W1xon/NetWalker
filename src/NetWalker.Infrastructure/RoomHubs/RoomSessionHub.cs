using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using NetWalker.Application.Common.Interfaces.Persistence;
using NetWalker.Application.Services;

namespace NetWalker.Infrastructure.RoomHubs;
[Authorize]
public class RoomSessionHub : Hub
{
    private readonly IUserRepository _userRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IRoomHubNotifier _roomHubNotifier;
    private readonly IRoomChatRepository _roomChatRepository;
    
    public RoomSessionHub(IUserRepository userRepository,
        IRoomRepository roomRepository,
        IRoomHubNotifier roomHubNotifier,
        IRoomChatRepository roomChatRepository)
    {
        _userRepository = userRepository;
        _roomRepository = roomRepository;
        _roomHubNotifier = roomHubNotifier;
        _roomChatRepository = roomChatRepository;
    }
    
    public async Task Send(string roomCode, string message)
    {
        if (string.IsNullOrWhiteSpace(message)) return;

        var userId = GetUserId();
        if(!await IsUserFromRoom(roomCode, userId))
        {
            await _roomHubNotifier.NotifyCallerAsync(Context.ConnectionId, "You are not in this room.");
            return;
        }
        var user = await _userRepository.GetByIdAsync(userId);
        var nickname = user?.Nick ?? "Anonymous";
        await _roomChatRepository.AddMessage(roomCode, $"{nickname}: {message}");
        await _roomHubNotifier.NotifyMessageReceivedAsync(roomCode, $"{nickname}: {message}");
    }
    public async Task JoinGroup(string roomCode)
    {
        var userId = GetUserId();
        if(!await IsUserFromRoom(roomCode, userId))
        {
            await _roomHubNotifier.NotifyCallerAsync(Context.ConnectionId, "You are not in this room.");
            return;
        }
        await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);

        var messages = await _roomChatRepository.GetMessageFromChat(roomCode);
    
        if (messages is not null)
        {
            foreach (var msg in messages)
            {
                await _roomHubNotifier.NotifyCallerAsync(Context.ConnectionId, msg);
            }
        }
    }
    
    public async Task LeaveGroup(string roomCode)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomCode);
    }
    
    private async Task<bool> IsUserFromRoom(string roomCode, Guid userId)
    { 
        return userId != Guid.Empty && await IsUserInRoom(userId, roomCode);
    }
    private Guid GetUserId()
    {
        var strId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(strId, out var userId) ? userId : Guid.Empty;
    }

    private async Task<bool> IsUserInRoom(Guid userId, string roomCode)
    {
        var room = await _roomRepository.GetBySessionCodeAsync(roomCode);
        if(room is null) return false;
        if(room.ContainsPlayer(userId)) return true;
        return false;
    }
}