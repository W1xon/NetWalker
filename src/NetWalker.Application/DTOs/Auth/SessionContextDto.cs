namespace NetWalker.Application.DTOs.Auth;

public record SessionContextDto()
{
    public string IpAddress { get; init; } = string.Empty;
    public string DeviceType { get; init; } = string.Empty;
    public string Os { get; init; } = string.Empty;
    
}