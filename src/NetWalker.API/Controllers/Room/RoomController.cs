using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetWalker.Application.Common.Interfaces;
using NetWalker.Application.DTOs.Room;

namespace NetWalker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RoomController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    [HttpGet("{code:length(6)}/details")]
    public async Task<IActionResult> GetRoomDetails(string code, CancellationToken token)
    {
        var result = await _roomService.GetRoomDetailsAsync(code, token);
        
        if (!result.IsSuccess)
        {
            return Problem(
                detail: result.Error,
                statusCode: StatusCodes.Status404NotFound,
                title: "Get room details failed"
            );
        }
        
        return Ok(result.Value);
    }
    [HttpGet("{code:length(6)}")]
    public async Task<IActionResult> GetRoomByCode(string code, CancellationToken token)
    {
        var result = await _roomService.GetRoomByCodeAsync(code, token);

        if (!result.IsSuccess)
        {
            return Problem(
                detail: result.Error,
                statusCode: StatusCodes.Status404NotFound,
                title: "Get room failed"
            );
        }
        return Ok(result.Value);
    }

    [HttpGet("active-rooms")]
    public async Task<IActionResult> GetActiveRoom(CancellationToken token)
    {
        var result = await _roomService.GetActiveRoomsAsync(token);
        return Ok(result);
    }

    [HttpPost("join-room/{code:length(6)}")]
    public async Task<IActionResult> JoinToRoom(string code, CancellationToken  token)
    {
        var strId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(strId, out Guid id))
            return BadRequest("Некорректный Id пользователя");
        
        var result = await _roomService.JoinRoomAsync(id, code, token);
        
        if(!result.IsSuccess)
            return Problem(
                detail: result.Error,
                statusCode: StatusCodes.Status409Conflict,
                title: "Join to room failed"
            );
        
        return Ok();
    }
    [HttpPost("create-room")]
    public async Task<IActionResult> CreateRoom([FromBody] CreateRoomRequest request, CancellationToken token)
    {
        var strId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(strId, out Guid id))
            return BadRequest("Некорректный Id пользователя");

        var result = await _roomService.CreateRoomAsync(id, request, token);
        
        if(!result.IsSuccess)
            return Problem(
                detail: result.Error,
                statusCode: StatusCodes.Status409Conflict,
                title: "Create room failed"
            );
        
        return Ok(result.Value);
    }
}