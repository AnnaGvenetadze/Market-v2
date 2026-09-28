using Market.Extensions;
using Market.Services.Interfaces;
using Market.Services.Interfaces.Services;

namespace Market.Services;

public class AuthService /*: IAuthService*/
{
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public bool Login(string username, string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        bool isValidUser = _unitOfWork
            .AccountRepository
            .Search(a => 
                a.Username == username && 
                a.PasswordHash == password.ToHashCode() && 
                !a.IsDeleted)
            .Any();

        return isValidUser;
    }
}