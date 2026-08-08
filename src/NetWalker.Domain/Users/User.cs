namespace NetWalker.Domain;

public class User
{
    public Guid Id { get; private set; }
    public string Nick { get; private set; }
    public string PasswordHash { get; private set; }
    public DateTime CreatedTime { get; private set; }
    public PlayerStats Stats { get; private set; }

    private User() { }

    public User(string nick, string passwordHash)
    {
        Id = Guid.CreateVersion7();
        Nick = nick;
        PasswordHash = passwordHash;
        CreatedTime = DateTime.UtcNow;

        Stats = new PlayerStats(Id);
    }
    
    public void EnsureStatsInitialized()
    {
        Stats ??= new PlayerStats(Id);
    }
}