using NetWalker.Application.Common.Interfaces;
using NetWalker.Application.Common.Interfaces.Persistence;
using NetWalker.Application.Common.Models;
using NetWalker.Application.DTOs.Profile;

namespace NetWalker.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<Result<ProfileResponse>> GetProfileByNameAsync(Guid id, CancellationToken token = default)
    {
        var user = await _userRepository.GetByIdWithStatsAsync(id, token);
        if (user is null)
            return Result<ProfileResponse>.Failure("Нет такого профиля");

        if (user.Stats is null)
        { 
            user.EnsureStatsInitialized(); 
            await _userRepository.UpdateAsync(user, token);
        }
        var playerStats = new PlayerStatsResponse(user.Stats.TotalGames, user.Stats.TotalPlayTime, user.Stats.LongestSession);
        var profile = new ProfileResponse(user.Id, user.Nick, user.CreatedTime, playerStats);
        
        return Result<ProfileResponse>.Success(profile);
    }
    
    public async Task<Result<PlayerStatsResponse>> GetPlayerStatsByNameAsync(Guid id, CancellationToken token = default)
    {
        
        var user = await _userRepository.GetByIdWithStatsAsync(id, token);
        if (user is null)
            return Result<PlayerStatsResponse>.Failure("Нет такого профиля");
        if (user.Stats is null)
        {
            user.EnsureStatsInitialized();
            await _userRepository.UpdateAsync(user, token);
        }
        var playerStats = new PlayerStatsResponse(user.Stats.TotalGames, user.Stats.TotalPlayTime, user.Stats.LongestSession);
        
        return Result<PlayerStatsResponse>.Success(playerStats);
    }
}