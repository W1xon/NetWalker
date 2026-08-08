namespace NetWalker.Application.DTOs.Profile;

public record ProfileResponse(Guid Id, string Nick, DateTime CreatedTime, PlayerStatsResponse StatsResponse);