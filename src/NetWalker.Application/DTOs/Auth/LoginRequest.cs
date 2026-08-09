namespace NetWalker.Application.DTOs.Auth;

public record LoginRequest(string Name, string Password);
public record RegisterRequest(string Name, string Password);