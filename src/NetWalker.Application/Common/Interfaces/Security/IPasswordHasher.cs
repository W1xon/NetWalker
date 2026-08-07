namespace NetWalker.Application.Common.Interfaces.Security;

public interface IPasswordHasher
{
    bool Verify(string password, string hash);
    string Create(string password);
}