using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Market.DTO;
using Market.Services.Interfaces.Services;
using Microsoft.IdentityModel.Tokens;

namespace Market.Services;

public class TokenService : ITokenService
{
    private const string _key = "asjdh123!@#asdkjh456!@#asdkjh789!@#";
    private const string _issuer = "MarketApp";
    private const string _audience = "MarketApp";


    public string GenerateToken(AccountDTO user, IEnumerable<RoleDTO> Roles)
    {
        var claims = new[]
        {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };
        foreach (var role in Roles)
        {
            claims = claims.Append(new Claim(ClaimTypes.Role, role.Name)).ToArray();
        }
        ;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(3),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    public RefreshTokenDTO GenerateRefreshToken(int accountId)
    {
        return new RefreshTokenDTO
        {
            AccountId = accountId,
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            ExpiredAt = DateTime.UtcNow.AddDays(30),
            IsCancelled = false
        };
    }
    public ClaimsPrincipal? ValidateToken(string token)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));

        try
        {
            return new JwtSecurityTokenHandler().ValidateToken(token,
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _issuer,
                    ValidAudience = _audience,
                    IssuerSigningKey = key
                }, out _);
        }
        catch
        {
            return null;
        }
    }
    public static byte[] Hash(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        return SHA256.HashData(
            Encoding.UTF8.GetBytes(token));
    }
}