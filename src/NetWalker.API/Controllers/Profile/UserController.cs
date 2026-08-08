using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetWalker.API.DTOs;
using NetWalker.Application.Common.Interfaces;

namespace NetWalker.API.Controllers.Profile;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetProfile(CancellationToken token)
    {
        var strId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(strId, out Guid id))
            return BadRequest("Некорректный Id пользователя");
        var result = await _userService.GetProfileByNameAsync(id, token);
        if (!result.IsSuccess)
            return NotFound(result.Error);
        
        return Ok(result.Value);
    }
}