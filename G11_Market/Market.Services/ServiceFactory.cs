using Market.Services.Interfaces;
using Market.Services.Interfaces.Services;
using Serilog;

namespace Market.Services;

public sealed class ServiceFactory
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger _logger;
    private readonly CurrentUserContext _currentUserContext;
    private readonly AuthorizationService _authorizationService;
    private readonly Lazy<IEmployeeService> _employeeService;
    //private readonly Lazy<ISaleService> _saleService;

    public ServiceFactory(
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(unitOfWork);
        ArgumentNullException.ThrowIfNull(tokenService);
        ArgumentNullException.ThrowIfNull(logger);

        _unitOfWork = unitOfWork;
        _logger = logger;

        _currentUserContext = 
            new CurrentUserContext(unitOfWork, tokenService);
        _authorizationService = 
            new AuthorizationService(_currentUserContext, logger);

        _employeeService = new Lazy<IEmployeeService>(() =>
            AuthorizationProxy<IEmployeeService>.Wrap(
                new EmployeeService(_unitOfWork, _logger),
                _authorizationService));

        //_saleService = new Lazy<ISaleService>(() =>
        //    AuthorizationProxy<ISaleService>.Wrap(
        //        new SaleService(
        //            _unitOfWork,
        //            _logger,
        //            _currentUserContext),
        //        _authorizationService));
    }

    public IEmployeeService EmployeeService =>
        _employeeService.Value;

    //public ISaleService SaleService =>
    //    _saleService.Value;

    public void SetAccessToken(string accessToken)
    {
        _currentUserContext.SetAccessToken(accessToken);
    }

    public void ClearAuthentication()
    {
        _currentUserContext.ClearAuthentication();
    }
}
