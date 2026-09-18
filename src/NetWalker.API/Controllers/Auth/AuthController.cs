using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetWalker.Application.Common.Interfaces;
using NetWalker.Application.DTOs.Auth;

namespace NetWalker.API.Controllers.Auth;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    [HttpGet("me")]
    [Authorize] 
    public IActionResult GetCurrentUser()
    {
        return Ok(new { name = User.Identity?.Name });
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(loginRequest, cancellationToken);
        if (!result.IsSuccess)
        {
            return Problem(
                detail: result.Error,
                statusCode: StatusCodes.Status401Unauthorized,
                title: result.Error
            );
        }
        AddSecureCookie(result.Value.Token);
        return Ok(result.Value);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest, CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(registerRequest, cancellationToken);

        if (!result.IsSuccess)
        {
            return Problem(
                detail: result.Error,
                statusCode: StatusCodes.Status409Conflict,
                title: "Registration failed"
            );
        }
        AddSecureCookie(result.Value.Token);
        return Ok(result.Value);
    }
    
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("jwt", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        });
    
        return Ok(new { message = "Logged out" });
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordRequest changePasswordRequest, CancellationToken token)
    {
        var strId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(strId, out Guid id))
            return BadRequest("Некорректный Id пользователя");
        
        var result =await _authService.ChangePassword(changePasswordRequest,id, token);
        
        if(!result.IsSuccess)
            return Problem(
            detail: result.Error,
            statusCode: StatusCodes.Status400BadRequest,
            title: "Change password failed"
        );
        
        AddSecureCookie(result.Value.Token);
        return Ok(result.Value);
    }
        
    private void AddSecureCookie(string token) =>
        HttpContext.Response.Cookies.Append("jwt", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddHours(1)
        });
}