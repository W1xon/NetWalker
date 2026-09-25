namespace NetWalker.Domain.Rooms;

public class RoomChat
{
    public Guid Id { get; init; }
    public string Code { get; private set; }
    private readonly List<string> _messages = new();
    public IReadOnlyList<string> Messages => _messages;
    
    
    private RoomChat(){}
    
    public RoomChat(string code)
    {
        Id = Guid.CreateVersion7();
        Code = code;
    }

    public void AddMessage(string text)
    {
        _messages.Add(text);
    }
}