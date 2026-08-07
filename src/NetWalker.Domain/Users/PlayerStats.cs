namespace NetWalker.Domain;

public class PlayerStats
{
    public Guid UserId { get; private set; }
    public int TotalGames { get; private set; }
    public int TotalPlayTime { get; private set; }
    public int LongestSession { get; private set; }
}