using NetWalker.Domain;
using NetWalker.Domain.Users;

namespace NetWalker.Application.Common.Interfaces.Security;

public interface IJwtProvider
{
    string GenerateToken(User user);
}