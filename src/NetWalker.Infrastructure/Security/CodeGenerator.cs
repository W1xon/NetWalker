using System.Buffers;
using NetWalker.Application.Common.Interfaces.Security;

namespace NetWalker.Infrastructure.Security;

public class CodeGenerator : ICodeGenerator
{
    private const string Chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
    
    public string Generate()
    {
        Span<char> chars = stackalloc char[6];
        for (int i = 0; i < 6; i++)
        {
            chars[i] = Chars[Random.Shared.Next(Chars.Length)];
        }

        return new string(chars);
    }
}