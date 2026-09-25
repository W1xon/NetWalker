namespace NetWalker.Application.Common.Interfaces.Persistence;

public interface IRoomChatRepository
{
    Task AddMessage(string code, string text, CancellationToken cancellationToken = default);
    Task DeleteChat(string code, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>?> GetMessageFromChat(string code, CancellationToken cancellationToken = default);
}