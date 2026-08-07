using Microsoft.AspNetCore.Authorization;

namespace NetWalker.API.Endpoints;

public static class PageEndpoints
{
    public static IEndpointRouteBuilder MapPageEndpoints(this IEndpointRouteBuilder app)
    {
        var pages = app.MapGroup("");

        pages.MapGet("/auth", (IWebHostEnvironment env) =>
            Results.File(Path.Combine(env.WebRootPath, "pages/auth.html"), "text/html"));
        pages.MapGet("/", (IWebHostEnvironment env) =>
            Results.File(Path.Combine(env.WebRootPath, "pages/index.html"), "text/html"));
        pages.MapGet("/dashboard", (HttpContext context, IWebHostEnvironment env)  =>
        {
            if (context.User.Identity?.IsAuthenticated != true)
            {
                return Results.Redirect("/auth");
            }
            return Results.File(Path.Combine(env.WebRootPath, "pages/dashboard.html"), "text/html");
        });

        app.MapFallback((IWebHostEnvironment env) => 
            Results.File(Path.Combine(env.WebRootPath, "pages/404.html"), "text/html"));
        
        return app;
    }
}