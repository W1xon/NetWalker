using Microsoft.AspNetCore.Mvc;
using NetWalker.API.DTOs;
using NetWalker.Application.Common.Interfaces;

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
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthRequest authRequest, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(authRequest.Name, authRequest.Password, cancellationToken);
        if (!result.IsSuccess)
        {
            return Problem(
                detail: result.Error,
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Login failed"
            );
        }
        AddSecureCookie(result.Value.Token);
        return Ok(result.Value);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] AuthRequest authRequest, CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(authRequest.Name, authRequest.Password, cancellationToken);

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

    private void AddSecureCookie(string token) =>
        HttpContext.Response.Cookies.Append("jwt", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddHours(1)
        });
}