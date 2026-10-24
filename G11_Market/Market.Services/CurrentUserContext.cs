using Market.Services.Enums;
using Market.Services.Interfaces;
using Market.Services.Interfaces.Services;
using System.Security.Claims;

namespace Market.Services;

internal sealed class CurrentUserContext : ICurrentUserContext
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private string? _accessToken;
    private int? _employeeId;

    public CurrentUserContext(IUnitOfWork unitOfWork, ITokenService tokenService)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
    }

    public int EmployeeId
    {
        get
        {
            if (!IsAuthenticated || !_employeeId.HasValue)
                throw new UnauthorizedAccessException("User is not authenticated.");

            return _employeeId.Value;
        }
    }


    private ClaimsPrincipal? User
    {
        get
        {
            if (_accessToken == null)
            {
                return null;
            }

            return _tokenService.ValidateToken(_accessToken);
        }
    }


    public bool IsAuthenticated =>
        User?.Identity?.IsAuthenticated == true;


    public bool HasRole(Role role)
    {
        var user = User;
        if (user == null || !IsAuthenticated)
            return false;

        string roleName = role switch
        {
            Role.Cashier => "Cashier",
            Role.InventoryManager => "Inventory Manager",
            Role.Administrator => "Administrator",
            _ => throw new ArgumentOutOfRangeException(nameof(role))
        };

        return user.IsInRole(roleName);
    }


    public void SetAccessToken(string accessToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(accessToken);

        ClearAuthentication();

        var user = _tokenService.ValidateToken(accessToken);

        if (user?.Identity?.IsAuthenticated != true)
            throw new UnauthorizedAccessException("Invalid access token.");

        int accountId = GetAccountId(user);
        int employeeId = GetEmployeeId(accountId);

        _accessToken = accessToken;
        _employeeId = employeeId;
    }


    public void ClearAuthentication()
    {
        _accessToken = null;
        _employeeId = null;
    }


    private static int GetAccountId(ClaimsPrincipal user)
    {
        var accountIdClaim =
            user.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? user.FindFirst("sub")?.Value;

        if (!int.TryParse(accountIdClaim, out int accountId) || accountId <= 0)
            throw new UnauthorizedAccessException("Invalid account identity.");

        return accountId;
    }


    private int GetEmployeeId(int accountId)
    {
        var account = _unitOfWork.AccountRepository.GetById(accountId);
        var employee = _unitOfWork.EmployeeRepository.GetByAccountId(accountId);

        if (account == null || employee == null || employee.IsDeleted ||
            account.IsDeleted || !account.IsActive)
            throw new UnauthorizedAccessException("Account or employee is inactive.");

        return employee.Id;
    }
}