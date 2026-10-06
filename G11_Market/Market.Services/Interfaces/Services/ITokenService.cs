using Market.DTO;

namespace Market.Services.Interfaces.Services;

public interface ITokenService
{
    string GenerateToken(
        AccountDTO user,
        IEnumerable<RoleDTO> roles);

    RefreshTokenDTO GenerateRefreshToken(int accountId);
}