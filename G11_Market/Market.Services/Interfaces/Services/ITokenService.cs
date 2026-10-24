using Market.DTO;
using System.Security.Claims;

namespace Market.Services.Interfaces.Services;

public interface ITokenService
{
    string GenerateToken(
        AccountDTO user,
        IEnumerable<RoleDTO> roles);

    RefreshTokenDTO GenerateRefreshToken(int accountId);
    ClaimsPrincipal? ValidateToken(string token);
}