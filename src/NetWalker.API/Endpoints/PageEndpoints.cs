using Microsoft.AspNetCore.Authorization;

namespace NetWalker.API.Endpoints;

public static class PageEndpoints
{
    public static IEndpointRouteBuilder MapPageEndpoints(this IEndpointRouteBuilder app)
    {
        var pages = app.MapGroup("");

        pages.MapGet("/auth", () =>
            Results.File(Path.Combine(app.ServiceProvider.GetRequiredService<IWebHostEnvironment>().WebRootPath, "pages/auth.html"), "text/html"));
        
        pages.MapGet("/", (HttpContext context, IWebHostEnvironment env)  =>
        {
            if (context.User.Identity?.IsAuthenticated != true)
            {
                return Results.Redirect("/auth");
            }
            return Results.File(Path.Combine(env.WebRootPath, "pages/index.html"), "text/html");
        });

        app.MapFallback(() => Results.Redirect("/"));
        return app;
    }
}