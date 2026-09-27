using System.IdentityModel.Tokens.Jwt;
using NetWalker.Application.Common.Interfaces.Security;

namespace NetWalker.API.Middlewares;

public class TokenRefreshMiddleware
{
    private readonly RequestDelegate _next;

    public TokenRefreshMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IUserSessionService userSessionService)
    {
        var isAccessValid = false;
        if(context.Request.Cookies.TryGetValue("jwt_access", out var accessToken) 
           && !string.IsNullOrWhiteSpace(accessToken))
        {
            isAccessValid = IsTokenValid(accessToken);
        }
        var hasRefresh = context.Request.Cookies.TryGetValue("refreshToken", out var refreshToken);

        if (!isAccessValid && hasRefresh && !string.IsNullOrWhiteSpace(refreshToken))
        {
            var result = await userSessionService.RefreshSessionAsync(refreshToken);
            if (result.IsSuccess)
            {
                context.Response.Cookies.Append("jwt_access", result.Value.AccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = context.Request.IsHttps,
                    SameSite = SameSiteMode.Strict,
                    Path = "/",
                    Expires = DateTimeOffset.UtcNow.AddMinutes(15)
                });

                context.Response.Cookies.Append("refreshToken", result.Value.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = context.Request.IsHttps,
                    SameSite = SameSiteMode.Strict,
                    Path = "/",
                    Expires = DateTimeOffset.UtcNow.AddDays(7)
                });

                context.Items["jwt_access"] = result.Value.AccessToken;
            }
        }

        await _next(context);
    }
    
    private bool IsTokenValid(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token))
                return false;
            
            var jwtToken = handler.ReadJwtToken(token);
            return jwtToken.ValidTo > DateTime.UtcNow;
        }
        catch
        {
            return false;
        }
    }
}