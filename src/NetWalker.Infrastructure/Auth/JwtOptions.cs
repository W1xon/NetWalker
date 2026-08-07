using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace NetWalker.Infrastructure.Auth;

public class JwtOptions
{
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string Algorithm { get; set; } = SecurityAlgorithms.HmacSha256;
    public int ExpiryMinutes { get; set; }
    public SecurityKey GetSymmetricSecurityKey() => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));
}