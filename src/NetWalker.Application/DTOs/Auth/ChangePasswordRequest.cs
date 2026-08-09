namespace NetWalker.Application.DTOs.Auth;

public record ChangePasswordRequest(string OldPassword, string NewPassword);