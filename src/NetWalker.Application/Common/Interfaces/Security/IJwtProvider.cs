using NetWalker.Domain;

namespace NetWalker.Application.Common.Interfaces.Security;

public interface IJwtProvider
{
    string GenerateToken(User user);
}