namespace NetWalker.Domain.Entities;

public class UserSession
{
    public Guid Id { get; private set; } = Guid.CreateVersion7(); 
    
    public Guid UserId { get; private set; }
    
    public string Token { get; private set; } = string.Empty; 
    
    public DateTime CreatedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public bool IsRevoked { get; private set; }
    
    private UserSession() { }
    
    public UserSession(Guid userId, string refreshToken, TimeSpan lifetime)
    {
        UserId = userId;
        Token = refreshToken;
        CreatedAt = DateTime.UtcNow;
        ExpiresAt = CreatedAt.Add(lifetime);
        IsRevoked = false;
    }

    public bool IsActive => !IsRevoked && DateTime.UtcNow < ExpiresAt;

}