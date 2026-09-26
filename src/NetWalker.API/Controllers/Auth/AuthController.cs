using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetWalker.Application.Common.Interfaces;
using NetWalker.Application.Common.Interfaces.Security;
using NetWalker.Application.DTOs.Auth;
using UAParser.Interfaces;

namespace NetWalker.API.Controllers.Auth;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserSessionService _userSessionService;
    private readonly IUserAgentParser _userAgentParser;
    public AuthController(IAuthService authService,
        IUserSessionService userSessionService,
        IUserAgentParser userAgentParser)
    {
        _authService = authService;
        _userSessionService = userSessionService;
        _userAgentParser = userAgentParser;
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
        var sessionContext = CreateSessionContext();
        var result = await _authService.LoginAsync(loginRequest, sessionContext, cancellationToken);
        if (!result.IsSuccess)
        {
            return Problem(
                detail: result.Error,
                statusCode: StatusCodes.Status401Unauthorized,
                title: result.Error
            );
        }
        AddSecureCookie(result.Value.AccessToken);
        AddSecureCookie(result.Value.RefreshToken, "refreshToken", 7);
        return Ok(result.Value);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest, CancellationToken cancellationToken)
    {
        var sessionContext = CreateSessionContext();
        var result = await _authService.RegisterAsync(registerRequest, sessionContext, cancellationToken);

        if (!result.IsSuccess)
        {
            return Problem(
                detail: result.Error,
                statusCode: StatusCodes.Status409Conflict,
                title: "Registration failed"
            );
        }
        AddSecureCookie(result.Value.AccessToken);
        AddSecureCookie(result.Value.RefreshToken, "refreshToken", 7);
        return Ok(result.Value);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (!string.IsNullOrEmpty(refreshToken))
        {
            await _userSessionService.RevokeSessionAsync(refreshToken);
        }
        
        Response.Cookies.Delete("jwt_access", new CookieOptions
        {
            HttpOnly = true,
            Secure = HttpContext.Request.IsHttps,
            SameSite = SameSiteMode.Strict,
            Path = "/"
        });

        Response.Cookies.Delete("refreshToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = HttpContext.Request.IsHttps,
                SameSite = SameSiteMode.Strict,
                Path = "/"
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

        var sessionContext = CreateSessionContext();
        var result = await _authService.ChangePassword(changePasswordRequest,id, sessionContext, token);

        if(!result.IsSuccess)
            return Problem(
            detail: result.Error,
            statusCode: StatusCodes.Status400BadRequest,
            title: "Change password failed"
        );

        AddSecureCookie(result.Value.AccessToken);
        return Ok(result.Value);
    }

    private SessionContextDto CreateSessionContext()
    {
        var clientInfo = _userAgentParser.ClientInfo;
        return new SessionContextDto()
        {
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            DeviceType = clientInfo.Device.Family,
            Os = $"{clientInfo.OS.Family} {clientInfo.OS.Major}".Trim()
        };
    }
    private void AddSecureCookie(string token, string name = "jwt_access", int days = 0)
    {
        var expires = days > 0
            ? DateTimeOffset.UtcNow.AddDays(days)
            : DateTimeOffset.UtcNow.AddMinutes(15);

        HttpContext.Response.Cookies.Append(name, token, new CookieOptions
        {
            HttpOnly = true,
            Secure = HttpContext.Request.IsHttps,
            SameSite = SameSiteMode.Strict,
            Expires = expires,
            Path = "/",
        });
    }
}
