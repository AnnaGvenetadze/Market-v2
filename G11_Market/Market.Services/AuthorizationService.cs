using Market.Services.Enums;
using Market.Services.Interfaces;
using Serilog;
using System.Reflection;

namespace Market.Services;

public class AuthorizationService
{
    private readonly ICurrentUserContext _currentUserContext;
    private readonly ILogger _logger;

    public AuthorizationService(ICurrentUserContext currentUserContext, ILogger logger)
    {
        _currentUserContext = currentUserContext ?? throw new ArgumentNullException(nameof(currentUserContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }


    public void RequireAnyRole(string methodName, IReadOnlyCollection<Role> roles)
    {

        ArgumentNullException.ThrowIfNull(roles);
        if (roles.Count == 0)
            throw new ArgumentException("At least one role is required.", nameof(roles));
        ThrowIfNotAuthenticated(methodName);
        
        if (!roles.Any(_currentUserContext.HasRole))
        {
            _logger.Warning($"Access denied to method: {methodName}");
            throw new UnauthorizedAccessException("Access denied.");
        }
    }


    public void ThrowIfNotAuthenticated(string methodName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(methodName);

        if (!_currentUserContext.IsAuthenticated)
        {
            _logger.Warning($"Unauthorized access to method: {methodName}");
            throw new UnauthorizedAccessException("User isn't authenticated.");
        }
    }
}