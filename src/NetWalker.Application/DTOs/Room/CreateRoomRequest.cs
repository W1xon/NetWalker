namespace NetWalker.Application.DTOs.Room;

public record CreateRoomRequest(int MaxPlayers, bool FromCli = false);
