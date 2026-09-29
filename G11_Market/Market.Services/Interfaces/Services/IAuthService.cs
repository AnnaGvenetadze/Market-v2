using Market.DTO;

namespace Market.Services.Interfaces.Services;

public interface IAuthService
{
    Task<LoginResultDTO> Login(string username, string password, CancellationToken cancellationToken = default);
}