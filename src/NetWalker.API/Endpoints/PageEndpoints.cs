namespace NetWalker.API.Endpoints;

public static class PageEndpoints
{
    public static IEndpointRouteBuilder MapPageEndpoints(this IEndpointRouteBuilder app)
    {
        var pages = app.MapGroup("");
        pages.MapGet("auth/login", () =>
            Results.File(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/pages/login.html"), "text/html"));
        
        pages.MapGet("/", () => 
            Results.File(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/pages/index.html"), "text/html"));
        return app;
    }
}