using NetWalker.Application.Common.Interfaces.Security;
namespace NetWalker.Infrastructure.Security;

public class PasswordHasher : IPasswordHasher
{
    private const int WORKFACTOR = 12;
    public bool Verify(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
    public string Create(string password) => BCrypt.Net.BCrypt.HashPassword(password, WORKFACTOR);
}