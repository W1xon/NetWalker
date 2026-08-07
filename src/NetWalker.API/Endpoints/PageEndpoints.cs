using Microsoft.AspNetCore.Authorization;

namespace NetWalker.API.Endpoints;

public static class PageEndpoints
{
    public static IEndpointRouteBuilder MapPageEndpoints(this IEndpointRouteBuilder app)
    {
        var pages = app.MapGroup("");

        pages.MapGet("/auth/login", () =>
            Results.File(Path.Combine(app.ServiceProvider.GetRequiredService<IWebHostEnvironment>().WebRootPath, "pages/login.html"), "text/html"));

        pages.MapGet("/", [Authorize] () => 
            Results.File(Path.Combine(app.ServiceProvider.GetRequiredService<IWebHostEnvironment>().WebRootPath, "pages/index.html"), "text/html"));

        app.MapFallback(context =>
        {
            context.Response.Redirect("/");
            return Task.CompletedTask;
        });
        return app;
    }
}