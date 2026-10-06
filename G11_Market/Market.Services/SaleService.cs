using Market.DTO;
using Market.DTO.Enums;
using Market.Services.Interfaces;
using Serilog;

namespace Market.Services;

public class SaleService : ISaleService
{
    private readonly ILogger _logger;
    private readonly IUnitOfWork _unitOfWork;

    public SaleService(IUnitOfWork unitOfWork, ILogger logger)
    {
        _unitOfWork = unitOfWork
            ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));
    }

    public int CreateSale()
    {
        // employeeId-ის წყარო ჯერ არ გვაქვს.
        // ამას Auth/Session-ის გადაწყვეტის შემდეგ დავასრულებთ.

        throw new NotImplementedException();
    }

    public SaleDTO? GetById(int saleId)
    {
        if (saleId <= 0)
            throw new ArgumentOutOfRangeException(nameof(saleId));

        return _unitOfWork.SaleRepository.GetById(saleId);
    }

    public void CompleteSale(int saleId)
    {
        if (saleId <= 0)
            throw new ArgumentOutOfRangeException(nameof(saleId));

        var sale = _unitOfWork.SaleRepository.GetById(saleId);

        if (sale == null)
        {
            _logger.Warning(
                "Complete sale failed: SaleId {SaleId} does not exist",
                saleId);

            throw new InvalidOperationException($"Sale with ID {saleId} does not exist.");
        }

        _unitOfWork.SaleRepository.CompleteSale(saleId);
    }

    public void CancelSale(int saleId, string cancelReason)
    {
        // employeeId-ის წყარო ჯერ არ გვაქვს.
        // ამას Auth/Session-ის გადაწყვეტის შემდეგ დავასრულებთ.

        throw new NotImplementedException();
    }

    public void AddItem(int saleId, int productId, int quantity)
    {
        if (saleId <= 0)
            throw new ArgumentOutOfRangeException(nameof(saleId));
        if (productId <= 0)
            throw new ArgumentOutOfRangeException(nameof(productId));
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity));

        var sale = _unitOfWork.SaleRepository.GetById(saleId);

        if (sale == null)
        {
            _logger.Warning(
                "Add sale item failed: SaleId {SaleId} does not exist",
                saleId);

            throw new InvalidOperationException($"Sale with ID {saleId} does not exist.");
        }

        var product = _unitOfWork.ProductRepository.GetById(productId);

        if (product == null || product.IsDeleted)
        {
            _logger.Warning(
                "Add sale item failed: ProductId {ProductId} does not exist or is deleted",
                productId);

            throw new InvalidOperationException($"Product with ID {productId} does not exist.");
        }

        var saleItem = new SaleItemDTO
        {
            SaleId = saleId,
            ProductId = productId,
            Quantity = quantity,
            UnitPrice = product.Price
        };

        _unitOfWork.SaleItemRepository.Insert(saleItem);
    }

    public void UpdateItemQuantity(int saleItemId, int quantity)
    {
        if (saleItemId <= 0)
            throw new ArgumentOutOfRangeException(nameof(saleItemId));
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity));

        var saleItem = _unitOfWork.SaleItemRepository.GetById(saleItemId);

        if (saleItem == null)
        {
            _logger.Warning(
                "Update sale item failed: SaleItemId {SaleItemId} does not exist",
                saleItemId);

            throw new InvalidOperationException($"Sale item with ID {saleItemId} does not exist.");
        }

        _unitOfWork.SaleItemRepository.UpdateQuantity(saleItemId, quantity);
    }

    public void RemoveItem(int saleItemId)
    {
        if (saleItemId <= 0)
            throw new ArgumentOutOfRangeException(nameof(saleItemId));

        var saleItem = _unitOfWork.SaleItemRepository.GetById(saleItemId);

        if (saleItem == null)
        {
            _logger.Warning(
                "Remove sale item failed: SaleItemId {SaleItemId} does not exist",
                saleItemId);

            throw new InvalidOperationException($"Sale item with ID {saleItemId} does not exist.");
        }

        _unitOfWork.SaleItemRepository.Delete(saleItemId);
    }

    public IEnumerable<SaleDTO> GetByEmployee(int employeeId)
    {
        if (employeeId <= 0)
            throw new ArgumentOutOfRangeException(nameof(employeeId));

        return _unitOfWork.SaleRepository
            .GetSalesByEmployee(employeeId)
            .ToList();
    }

    public IEnumerable<SaleDTO> GetByStatus(SaleStatus status)
    {
        if (!Enum.IsDefined(status))
            throw new ArgumentOutOfRangeException(nameof(status));

        return _unitOfWork.SaleRepository
            .GetSalesByStatus(status)
            .ToList();
    }

    public IEnumerable<SaleDTO> GetByDateRange(DateTime from, DateTime to)
    {
        if (from > to)
            throw new ArgumentException("From date cannot be greater than to date.");

        return _unitOfWork.SaleRepository
            .GetSalesByDateRange(from, to)
            .ToList();
    }

    public decimal GetDailyIncome(DateTime date)
    {
        var from = date.Date;
        var to = from.AddDays(1);

        return _unitOfWork.SaleRepository
            .GetIncomeByDateRange(from, to);
    }
}