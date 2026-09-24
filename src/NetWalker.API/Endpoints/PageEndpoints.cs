namespace NetWalker.API.Endpoints;

public static class PageEndpoints
{
    public static IEndpointRouteBuilder MapPageEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/", (IWebHostEnvironment env) =>
        Results.File(Path.Combine(env.WebRootPath, "pages/index.html"), "text/html"));
        app.MapGet("/auth", (IWebHostEnvironment env) =>
            Results.File(Path.Combine(env.WebRootPath, "pages/auth.html"), "text/html"));
    
        var protectedPages = app.MapGroup("")
            .RequireAuthorization();
    
        protectedPages.MapGet("/dashboard", (IWebHostEnvironment env) =>
            Results.File(Path.Combine(env.WebRootPath, "pages/dashboard.html"), "text/html"));
    
        protectedPages.MapGet("/room/{code}", (IWebHostEnvironment env) =>
            Results.File(Path.Combine(env.WebRootPath, "pages/room.html"), "text/html"));
    
        protectedPages.MapGet("/profile", (IWebHostEnvironment env) =>
            Results.File(Path.Combine(env.WebRootPath, "pages/profile.html"), "text/html"));
    
        app.MapFallback((IWebHostEnvironment env) =>
            Results.File(Path.Combine(env.WebRootPath, "pages/404.html"), "text/html"));
    
    
        return app;
    }
}
