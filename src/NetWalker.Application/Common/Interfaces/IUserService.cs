using NetWalker.Application.Common.Models;
using NetWalker.Application.DTOs.Profile;

namespace NetWalker.Application.Common.Interfaces;

public interface IUserService
{
    Task<Result<ProfileResponse>> GetProfileByNameAsync(Guid id, CancellationToken token = default);
    Task<Result<PlayerStatsResponse>> GetPlayerStatsByNameAsync(Guid id, CancellationToken token = default);
    
}