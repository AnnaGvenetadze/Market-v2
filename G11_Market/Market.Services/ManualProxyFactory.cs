using Market.Services.Interfaces;
using Market.Services.Interfaces.Services;
using Serilog;

namespace Market.Services;

// Temporary manual composition for the WinForms authorization check.
// Replace with DI registration when the application's service setup is ready.
public static class ManualProxyFactory
{
    public static IEmployeeService CreateEmployeeService(
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        string accessToken,
        ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(unitOfWork);
        ArgumentNullException.ThrowIfNull(tokenService);
        ArgumentException.ThrowIfNullOrWhiteSpace(accessToken);
        ArgumentNullException.ThrowIfNull(logger);

        var currentUserContext = new CurrentUserContext(unitOfWork, tokenService);
        currentUserContext.SetAccessToken(accessToken);

        var authorizationService = new AuthorizationService(currentUserContext, logger);
        var employeeService = new EmployeeService(unitOfWork, logger);

        return AuthorizationProxy<IEmployeeService>.Wrap(
            employeeService,
            authorizationService);
    }
}
