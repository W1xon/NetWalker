using NetWalker.Domain.Rooms;

namespace NetWalker.Application.DTOs.Room;

public record GetRoomResponse(RoomStatus Status, string SessionCode, int MaxPlayers, DateTime CreatedTime);