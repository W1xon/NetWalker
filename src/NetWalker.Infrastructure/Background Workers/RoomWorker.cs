using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetWalker.Application.Common.Interfaces;
using NetWalker.Application.Common.Interfaces.Persistence;
using NetWalker.Domain.Rooms;

namespace NetWalker.Infrastructure.Background_Workers;

public class RoomWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public RoomWorker(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(30));
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                
                using var scope = _scopeFactory.CreateScope();
                
                var roomService = scope.ServiceProvider.GetRequiredService<IRoomService>();
                var now = DateTime.UtcNow;
                var rooms = await roomService.GetActiveRoomsAsync(stoppingToken);
                var activeRoomsList = rooms.ToList();
                var removeRooms = activeRoomsList.Where(r =>
                    r.Status == RoomStatus.Finished || r.Status == RoomStatus.Abandoned ||
                    (now - r.CreatedTime) > TimeSpan.FromDays(1)).ToList();
                
                
                Console.WriteLine($"Комнат для удаления: {removeRooms.Count()}");
                var roomRepository = scope.ServiceProvider.GetRequiredService<IRoomRepository>();
                var roomChatRepository = scope.ServiceProvider.GetRequiredService<IRoomChatRepository>();
                foreach (var room in removeRooms)
                {
                    await roomChatRepository.DeleteChat(room.SessionCode, stoppingToken); 
                    await roomRepository.Remove(room.SessionCode);
                }
            }
            catch
            {
                //TODO: Add logging library
                Console.WriteLine("Сервис по очистке комнат сломан");
            }
        }
    }
}