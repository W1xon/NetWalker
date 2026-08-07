namespace NetWalker.Domain;

public class Room
{
    public Guid Id { get; private set; }
    public string SessionCode { get; private set; }
    public Guid HostId { get; private set; }
    public int PlayerCount { get; private set; }
    public int MaxPlayers { get; private set; }
    public DateTime CreatedTime { get; private set; }
    public DateTime LastHeartbeat { get; private set; }
}