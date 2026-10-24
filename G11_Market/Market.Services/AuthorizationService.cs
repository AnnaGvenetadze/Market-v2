using Market.Services.Interfaces;
using Market.Services.Enums;
using Serilog;

namespace Market.Services;

public class AuthorizationService
{
    private readonly ICurrentUserContext _currentUserContext;
    private readonly ILogger _logger;

    public AuthorizationService(
        ICurrentUserContext currentUserContext,
        ILogger logger)
    {
        _currentUserContext = currentUserContext
            ?? throw new ArgumentNullException(nameof(currentUserContext));

        _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));
    }

    public void RequireAnyRole(
        string methodName,
        IReadOnlyCollection<Role> roles)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(methodName);
        ArgumentNullException.ThrowIfNull(roles);

        if (roles.Count == 0)
            throw new ArgumentException(
                "At least one role is required.",
                nameof(roles));

        if (!_currentUserContext.IsAuthenticated)
        {
            _logger.Warning(
                "Unauthorized access to {Method}",
                methodName);

            throw new UnauthorizedAccessException(
                "User is not authenticated.");
        }

        bool hasPermission = roles.Any(
            role => _currentUserContext.HasRole(role));

        if (!hasPermission)
        {
            _logger.Warning(
                "Access denied to {Method}",
                methodName);

            throw new UnauthorizedAccessException(
                "Access denied.");
        }
    }
}