namespace NetWalker.Application.DTOs.Room;

public record CreateRoomResponse(Guid RoomId, string SessionCode, string? TicketCode);