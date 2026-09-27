namespace NetWalker.Domain.Rooms;

public enum RoomStatus
{
    WaitingHost = 0,
    Lobby = 1,
    InGame = 2,
    Finished = 3,
    Abandoned = 4
}

public class Room
{
    public Guid Id { get; init; }
    public string SessionCode { get; init; } = null!;
    public string? TicketCode { get; private set; }
    public bool IsTicketClaimed { get; private set; }
    public Guid HostId { get; private set; }
    public int MaxPlayers { get; private set; }
    public RoomStatus Status { get; private set; }
    public DateTime CreatedTime { get; init; }
    public DateTime LastHeartbeat { get; private set; }

    private readonly List<Guid> _playerIds = new();
    
    public IReadOnlyList<Guid> PlayerIds => _playerIds;

    private Room() { } 

    private Room(Guid hostId, int maxPlayers, string sessionCode, string? ticket = null, bool fromCli = false)
    {
        Id = Guid.CreateVersion7();
        SessionCode = sessionCode;
        TicketCode = ticket;
        HostId = hostId;
        MaxPlayers = maxPlayers;
        CreatedTime = DateTime.UtcNow;
        LastHeartbeat = DateTime.UtcNow;
        
        Status = fromCli ? RoomStatus.Lobby : RoomStatus.WaitingHost;
        _playerIds.Add(hostId);
    }

    public static Room CreateFromCli(Guid hostId, int maxPlayers, string sessionCode) 
        => new(hostId, maxPlayers, sessionCode, fromCli: true);

    public static Room CreateFromWeb(Guid hostId, int maxPlayers, string sessionCode, string ticket) 
        => new(hostId, maxPlayers, sessionCode, ticket, fromCli: false);

    public bool TryClaimHost(Guid hostId, string ticket)
    {
        if (IsTicketClaimed || Status != RoomStatus.WaitingHost) return false;
        if (HostId != hostId || TicketCode != ticket) return false;

        if (DateTime.UtcNow - CreatedTime > TimeSpan.FromMinutes(5))
        {
            Close(RoomStatus.Abandoned);
            return false;
        }
        
        IsTicketClaimed = true;
        Status = RoomStatus.Lobby;
        TouchHeartbeat();
        return true;
    }

    public bool TryAddPlayer(Guid playerId)
    {
        //TODO: нужен когда будет консольный клиент
        //if (Status != RoomStatus.Lobby) return false;
        if (_playerIds.Count >= MaxPlayers) return false;
        if (_playerIds.Contains(playerId)) return true;

        _playerIds.Add(playerId);
        TouchHeartbeat();
        return true;
    }

    public bool TryLeavePlayer(Guid playerId)
    {
        if (!_playerIds.Contains(playerId)) return false;

        if (playerId == HostId)
            Close();

        _playerIds.Remove(playerId);
        TouchHeartbeat();
        return true;
    }

    public void CheckAbandonment(TimeSpan timeout)
    {
        if (Status is RoomStatus.Finished or RoomStatus.Abandoned) 
            return;

        if (DateTime.UtcNow - LastHeartbeat > timeout)
        {
            Close(RoomStatus.Abandoned);
        }
    }
    public bool ContainsPlayer(Guid playerId) => _playerIds.Contains(playerId);

    public void Close(RoomStatus finalStatus = RoomStatus.Finished)
    {
        Status = finalStatus;
        TouchHeartbeat();
    }

    public void TouchHeartbeat() => LastHeartbeat = DateTime.UtcNow;
}