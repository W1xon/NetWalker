namespace NetWalker.Domain.Users;

public enum TokenValidationResult
{
    ValidCurrent,
    ValidGracePeriod,
    Invalid
}
public class UserSession
{
    public Guid Id { get; private set; } = Guid.CreateVersion7(); 
    
    public Guid UserId { get; private set; }
    
    public string TokenHash { get; set; } = string.Empty;
    public string PreviousTokenHash { get; private set; } = string.Empty;
    public DateTime PreviousTokenExpiresAt { get; private set; }

    public string DeviceType { get; private set; } = string.Empty;
    public string Os { get; private set; } = string.Empty;
    public string IpAddress { get; private set; } = string.Empty;
    
    public DateTime CreatedAt { get; private set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; private set; }
    
    private readonly TimeSpan _graceTime = TimeSpan.FromSeconds(3);
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

    public void RotateToken(string newTokenHash)
    {
        PreviousTokenHash = TokenHash;
        TokenHash = newTokenHash;
        PreviousTokenExpiresAt = DateTime.UtcNow.Add(_graceTime);
    }

    public TokenValidationResult CheckToken(string tokenHash)
    {
        if (!IsActive) return TokenValidationResult.Invalid;
        if (TokenHash == tokenHash) return TokenValidationResult.ValidCurrent;
        if (!string.IsNullOrWhiteSpace(PreviousTokenHash) &&
            PreviousTokenHash == tokenHash &&
            PreviousTokenExpiresAt > DateTime.UtcNow)
            return TokenValidationResult.ValidGracePeriod;

        return TokenValidationResult.Invalid;
    }
    public bool IsActive => !IsRevoked && DateTime.UtcNow < ExpiresAt;
    public void Revoke() => IsRevoked = true;
}