using Market.DTO;

namespace Market.Services.Interfaces.Services;

public interface IAuthService
{
    Task<AuthResultDTO> Login(string username, string password, CancellationToken cancellationToken = default);
    Task Register(string username, string password, string email, CancellationToken cancellationToken = default);
    Task Logout(CancellationToken cancellationToken = default);
}