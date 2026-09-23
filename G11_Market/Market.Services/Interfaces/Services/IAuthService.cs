namespace Market.Services.Interfaces.Services;

public interface IAuthService
{
    bool Login(string username, string password);
}