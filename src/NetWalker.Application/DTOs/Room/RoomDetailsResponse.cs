using NetWalker.Domain.Rooms;

namespace NetWalker.Application.DTOs.Room;

public record RoomDetailsResponse(string SessionCode, RoomStatus Status, int MaxPlayers, List<RoomPlayerDto> Players, DateTime CreatedTime);