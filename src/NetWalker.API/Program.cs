using DotNetEnv;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NetWalker.API.Endpoints;
using NetWalker.API.Filters;
using NetWalker.Application.Common.Interfaces;
using NetWalker.Application.Common.Interfaces.Persistence;
using NetWalker.Application.Common.Interfaces.Security;
using NetWalker.Application.Common.Validators;
using NetWalker.Application.Services;
using NetWalker.Application.Services.Auth;
using NetWalker.Infrastructure.Auth;
using NetWalker.Infrastructure.Background_Workers;
using NetWalker.Infrastructure.Persistence;
using NetWalker.Infrastructure.Security;

Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(nameof(JwtOptions)));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

builder.Services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
    .Configure<IOptions<JwtOptions>>((bearerOptions, jwtOptions) =>
    {
        var options = jwtOptions.Value;
        bearerOptions.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = options.Issuer,
            ValidateAudience = true,
            ValidAudience = options.Audience,
            ValidateLifetime = true,
            IssuerSigningKey = options.GetSymmetricSecurityKey(),
            ValidateIssuerSigningKey = true,
        };

        bearerOptions.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if(context.Request.Cookies.TryGetValue("jwt", out var token))
                {
                    context.Token = token;
                }

                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                var path = context.Request.Path.Value ?? string.Empty;
                if (!path.StartsWith("/api", StringComparison.OrdinalIgnoreCase))
                {
                    context.HandleResponse();
                    context.Response.Redirect("/auth");
                }

                return Task.CompletedTask;
            }
        };
    });

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseNpgsql(connectionString));

builder.Services.AddAuthorization();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtProvider, JwtProvider>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<ICodeGenerator, CodeGenerator>();

builder.Services.AddHostedService<RoomWorker>();

builder.Services.AddValidatorsFromAssemblyContaining<ChangePasswordRequestValidator>();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});



// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapPageEndpoints();
app.MapControllers();
app.Run();
