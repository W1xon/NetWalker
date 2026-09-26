namespace NetWalker.Domain.Entities;

public class UserSession
{
    public Guid Id { get; private set; } = Guid.CreateVersion7(); 
    
    public Guid UserId { get; private set; }
    
    public string TokenHash { get; set; } = string.Empty;

    public string DeviceType { get; private set; } = string.Empty;
    public string Os { get; private set; } = string.Empty;
    public string IpAddress { get; private set; } = string.Empty;
    
    public DateTime CreatedAt { get; private set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; private set; }
    
    private UserSession() { }
    
    public UserSession(Guid userId, string refreshTokenHash, string deviceType, string os, string ipAddress, TimeSpan lifetime)
    {
        UserId = userId;
        TokenHash = refreshTokenHash;
        DeviceType = deviceType;
        Os = os;
        IpAddress = ipAddress;
        CreatedAt = DateTime.UtcNow;
        ExpiresAt = CreatedAt.Add(lifetime);
        IsRevoked = false;
    }

    public bool IsActive => !IsRevoked && DateTime.UtcNow < ExpiresAt;
    public void Revoke() => IsRevoked = true;
}