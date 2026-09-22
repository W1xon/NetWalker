using Argon2Sharp;
using NetWalker.Application.Common.Interfaces.Security;

namespace NetWalker.Infrastructure.Security;

public class PasswordHasher : IPasswordHasher
{
    private readonly Argon2Parameters _baseParameters = Argon2Parameters.CreateBuilder()
        .WithType(Argon2Type.Argon2id)
        .WithMemorySizeKB(65536) 
        .WithIterations(4)
        .WithParallelism(4)
        .WithHashLength(32)
        .WithRandomSalt(16)
        .Build();

    public string Create(string password)
    {
        var parametersWithSalt = _baseParameters with { Salt = Argon2.GenerateSalt() };

        return Argon2PhcFormat.HashToPhcString(password, parametersWithSalt);
    }

    public bool Verify(string password, string hash)
    {
        var (isValid, _) = Argon2PhcFormat.VerifyPhcString(password, hash);
        return isValid;
    }
}