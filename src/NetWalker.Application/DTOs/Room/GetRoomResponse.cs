using NetWalker.Domain.Rooms;

namespace NetWalker.Application.DTOs.Room;

public record GetRoomResponse(RoomStatus Status, int MaxPlayers, DateTime CreatedTime);