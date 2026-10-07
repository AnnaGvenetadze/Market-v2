using Market.DTO;
using Market.Services.Interfaces;
using Serilog;
using Market.Services.Interfaces.Services;

namespace Market.Services;

public class StockMovementService : IStockMovementService
{
    private readonly ILogger _logger;
    private readonly IUnitOfWork _unitOfWork;
    //private readonly ICurrentUserContext _currentUserContext;


    public StockMovementService(IUnitOfWork unitOfWork, ILogger logger)//, ICurrentUserContext currentUserContext)
    {
        _unitOfWork = unitOfWork
            ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));
        //_currentUserContext = currentUserContext
            //?? throw new ArgumentNullException(nameof(currentUserContext));
    }


    public IEnumerable<StockMovementDTO> GetByProductId(int productId)
    {
        if (productId <= 0)
            throw new ArgumentOutOfRangeException(nameof(productId));

        return _unitOfWork.StockMovementRepository
            .GetByProductId(productId)
            .ToList();
    }


    public IEnumerable<StockMovementDTO> GetByDateRange(DateTime from, DateTime to)
    {
        if (from > to)
            throw new ArgumentException("From date cannot be greater than to date.");

        return _unitOfWork.StockMovementRepository
            .GetByDateRange(from, to)
            .ToList();
    }


    public IEnumerable<StockMovementDTO> GetByEmployeeId(int employeeId)
    {
        if (employeeId <= 0)
            throw new ArgumentOutOfRangeException(nameof(employeeId));

        return _unitOfWork.StockMovementRepository
            .GetByEmployeeId(employeeId)
            .ToList();
    }


    public IEnumerable<StockDTO> GetOutOfStockProducts()
    {
        return _unitOfWork.StockMovementRepository
            .GetOutOfStockProducts()
            .ToList();
    }


    public void Refill(int productId, int quantity)
    {
        if (productId <= 0)
            throw new ArgumentOutOfRangeException(nameof(productId));
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity));

        var product = _unitOfWork.ProductRepository.GetById(productId);

        if (product == null)
        {
            _logger.Warning(
                "Refill failed: ProductId {ProductId} does not exist",
                productId);

            throw new InvalidOperationException($"Product with ID {productId} does not exist.");
        }

        //var employeeId = _currentUserContext.EmployeeId;

        //_unitOfWork.StockMovementRepository.Refill(productId, quantity, employeeId);
    }


    public void Adjust(int productId, int quantityDifference, string? reason)
    {
        if (productId <= 0)
            throw new ArgumentOutOfRangeException(nameof(productId));
        if (quantityDifference == 0)
            throw new ArgumentOutOfRangeException(nameof(quantityDifference), "Quantity difference cannot be zero.");

        var product = _unitOfWork.ProductRepository.GetById(productId);

        if (product == null)
        {
            _logger.Warning(
                "Stock adjustment failed: ProductId {ProductId} does not exist",
                productId);

            throw new InvalidOperationException($"Product with ID {productId} does not exist.");
        }

        //var employeeId = _currentUserContext.EmployeeId;

        //_unitOfWork.StockMovementRepository.Adjust(productId, quantityDifference, employeeId, reason);
    }
}