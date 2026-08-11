namespace NetWalker.Application.DTOs.Room;

public record RoomPlayerDto(Guid Id, string Username, bool IsHost);