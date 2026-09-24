namespace NetWalker.Domain.Users;

public class PlayerStats
{
    public Guid UserId { get; private set; }
    public int TotalGames { get; private set; }
    public int TotalPlayTime { get; private set; }
    public int LongestSession { get; private set; }
    
    private PlayerStats() { }

    public PlayerStats(Guid userId)
    {
        UserId = userId;
        TotalGames = 0;
        TotalPlayTime = 0;
        LongestSession = 0;
    }
}