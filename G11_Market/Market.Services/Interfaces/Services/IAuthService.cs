using Market.DTO;

namespace Market.Services.Interfaces.Services;

public interface IAuthService
{
    Task<AuthResultDTO> Login(string username, string password, CancellationToken cancellationToken = default);
}