using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using NetWalker.Application.Common.Interfaces.Security;
using NetWalker.Domain;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NetWalker.Domain.Users;

namespace NetWalker.Infrastructure.Auth;

public class JwtProvider : IJwtProvider
{
    private readonly JwtOptions _options;

    public JwtProvider(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }
    public string GenerateToken(User user)
    {
        List<Claim> claims = [new Claim( ClaimTypes.Name, user.Nick), new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())];
        
        SigningCredentials signingCredentials = new SigningCredentials(_options.GetSymmetricSecurityKey(), _options.Algorithm);
        
        JwtSecurityToken jwt = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(_options.ExpiryMinutes)),
            signingCredentials: signingCredentials
            );
        
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}