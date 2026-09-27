using NetWalker.Application.Common.Models;
using NetWalker.Application.DTOs.Profile;

namespace NetWalker.Application.Common.Interfaces;

public interface IUserService
{
    Task<Result<ProfileResponse>> GetProfileByIdAsync(Guid id, CancellationToken token = default);
    Task<Result<PlayerStatsResponse>> GetPlayerStatsByIdAsync(Guid id, CancellationToken token = default);
    
}