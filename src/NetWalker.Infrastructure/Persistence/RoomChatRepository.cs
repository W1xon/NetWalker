using Microsoft.EntityFrameworkCore;
using NetWalker.Application.Common.Interfaces.Persistence;
using NetWalker.Domain.Rooms;

namespace NetWalker.Infrastructure.Persistence;

public class RoomChatRepository : IRoomChatRepository
{
    private readonly AppDbContext _context;

    public RoomChatRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task AddMessage(string code, string text, CancellationToken cancellationToken = default)
    {
        var chat = await _context.Chats.FirstOrDefaultAsync(c => c.Code == code);
        if (chat == null)
        {
            chat = new RoomChat(code);
            chat.AddMessage(text);
            _context.Chats.Add(chat);
        }
        else
        {
            chat.AddMessage(text);
            _context.Chats.Update(chat);
        }
        
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteChat(string code, CancellationToken cancellationToken = default)
    {
        await _context.Chats.Where(c => c.Code == code).ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<string>?> GetMessageFromChat(string code, CancellationToken cancellationToken = default)
    {
        return await _context.Chats
            .AsNoTracking()
            .Where(c => c.Code == code)
            .Select(c => c.Messages)
            .FirstOrDefaultAsync(cancellationToken);
    }
}