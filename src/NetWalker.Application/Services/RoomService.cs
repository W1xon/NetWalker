using NetWalker.Application.Common.Interfaces;
using NetWalker.Application.Common.Interfaces.Persistence;
using NetWalker.Application.Common.Interfaces.Security;
using NetWalker.Application.Common.Models;
using NetWalker.Application.DTOs.Room;
using NetWalker.Domain.Rooms;

namespace NetWalker.Application.Services;

public class RoomService : IRoomService
{
    private readonly ICodeGenerator _codeGenerator;
    private readonly IRoomRepository _roomRepository;
    public async Task<Result<CreateRoomResponse>> CreateRoomAsync(Guid hostId, CreateRoomRequest request, CancellationToken token = default)
    {
        var room = request.FromCli ? Room.CreateFromCli(hostId, request.MaxPlayers, _codeGenerator.Generate()):
            Room.CreateFromWeb(hostId, request.MaxPlayers, _codeGenerator.Generate(), _codeGenerator.Generate());
        await _roomRepository.AddAsync(room, token);

        return Result<CreateRoomResponse>.Success(new CreateRoomResponse(room.Id, room.SessionCode, room.TicketCode));
    }

    public async Task<Result<GetRoomResponse>> GetRoomByCodeAsync(string sessionCode, CancellationToken token = default)
    {
        var existingRoom = await _roomRepository.GetBySessionCodeAsync(sessionCode, token);

        if (existingRoom is null)
            return Result<GetRoomResponse>.Failure("Такой комнаты не существует");

        return Result<GetRoomResponse>.Success(new GetRoomResponse(existingRoom.Status, existingRoom.MaxPlayers,
            existingRoom.CreatedTime));
    }

    public async Task<IReadOnlyList<GetRoomResponse>> GetActiveRoomsAsync(CancellationToken token = default)
    {
        var activeRooms =  await _roomRepository.GetActiveRoomsAsync(token);

        return activeRooms
            .Select(room => new GetRoomResponse(room.Status, room.MaxPlayers, room.CreatedTime))
            .ToList();
    }

    public async Task<Result> JoinRoomAsync(Guid playerId, string sessionCode, CancellationToken token = default)
    {
        var existingRoom = await _roomRepository.GetBySessionCodeAsync(sessionCode, token);
        
        if(existingRoom is null)
            return Result.Failure("Такой комнаты не существует");
        if(!existingRoom.TryAddPlayer(playerId))
            return Result.Failure("Невозможно присоединиться к комнате");
        
        await _roomRepository.UpdateAsync(existingRoom, token);
        return Result.Success();
    }

    public async Task<Result> LeaveRoomAsync(Guid playerId, string sessionCode, CancellationToken token = default)
    {
        var existingRoom = await _roomRepository.GetBySessionCodeAsync(sessionCode, token);
        
        if(existingRoom is null)
            return Result.Failure("Такой комнаты не существует");
        if(!existingRoom.TryLeavePlayer(playerId))
            return Result.Failure("Вы не можете покинуть комнату, т.к. не состоите в ней");
        
        await _roomRepository.UpdateAsync(existingRoom, token);
        return Result.Success();
    }
}