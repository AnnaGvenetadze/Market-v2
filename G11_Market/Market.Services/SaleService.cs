using Market.DTO;
using Market.DTO.Enums;
using Market.Services.Interfaces;
using Serilog;

namespace Market.Services;

public class SaleService : ISaleService
{
    private readonly ILogger _logger;
    private readonly IUnitOfWork _unitOfWork;
    //private readonly ICurrentUserContext _currentUserContext;

    public SaleService(
        IUnitOfWork unitOfWork,
        ILogger logger/*,
        ICurrentUserContext currentUserContext*/)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        /*_currentUserContext = currentUserContext ?? throw new ArgumentNullException(nameof(currentUserContext));*/
    }


    public int CreateSale()
    {
        //var employeeId = _currentUserContext.EmployeeId;
        //if (employeeId <= 0)
        //    throw new InvalidOperationException("Authenticated employee was not found.");

        var sale = new SaleDTO
        {
            //CreatedEmployeeId = employeeId,
            Status = SaleStatus.Draft
        };

        return _unitOfWork.SaleRepository.Insert(sale);
    }


    public SaleDTO? GetById(int saleId)
    {
        if (saleId <= 0)
            throw new ArgumentOutOfRangeException(nameof(saleId));

        return _unitOfWork.SaleRepository.GetById(saleId);
    }


    public void CompleteSale(int saleId)
    {
        //var employeeId = _currentUserContext.EmployeeId;
        //if (employeeId <= 0)
        //    throw new InvalidOperationException("Authenticated employee was not found.");
        if (saleId <= 0)
            throw new ArgumentOutOfRangeException(nameof(saleId));

        var sale = _unitOfWork.SaleRepository.GetById(saleId);
        if (sale is null)
        {
            _logger.Warning(
                "Complete sale failed: SaleId {SaleId} does not exist",
                saleId);

            throw new InvalidOperationException($"Sale with ID {saleId} does not exist.");
        }

        _unitOfWork.SaleRepository.CompleteSale(saleId/*, employeeId*/);
    }


    public void CancelSale(int saleId, string cancelReason)
    {
        //var employeeId = _currentUserContext.EmployeeId;
        //if (employeeId <= 0)
        //    throw new InvalidOperationException("Authenticated employee was not found.");
        if (saleId <= 0)
            throw new ArgumentOutOfRangeException(nameof(saleId));
        ArgumentException.ThrowIfNullOrWhiteSpace(cancelReason);

        var sale = _unitOfWork.SaleRepository.GetById(saleId);
        if (sale is null)
        {
            _logger.Warning(
                "Cancel sale failed: SaleId {SaleId} does not exist",
                saleId);

            throw new InvalidOperationException($"Sale with ID {saleId} does not exist.");
        }

        //_unitOfWork.SaleRepository.CancelSale(saleId, employeeId, cancelReason.Trim());
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
        if (sale is null)
        {
            _logger.Warning(
                "Add sale item failed: SaleId {SaleId} does not exist",
                saleId);

            throw new InvalidOperationException($"Sale with ID {saleId} does not exist.");
        }

        var product = _unitOfWork.ProductRepository.GetById(productId);
        if (product is null)
        {
            _logger.Warning(
                "Add sale item failed: ProductId {ProductId} does not exist.",
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
        if (saleItem is null)
        {
            _logger.Warning(
                "Update sale item failed: SaleItemId {SaleItemId} does not exist",
                saleItemId);

            throw new InvalidOperationException($"Sale item with ID {saleItemId} does not exist.");
        }

        _unitOfWork.SaleItemRepository.UpdateQuantity(saleItemId,quantity);
    }


    public void RemoveItem(int saleItemId)
    {
        if (saleItemId <= 0)
            throw new ArgumentOutOfRangeException(nameof(saleItemId));

        var saleItem = _unitOfWork.SaleItemRepository.GetById(saleItemId);
        if (saleItem is null)
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

        return _unitOfWork.SaleRepository.GetSalesByEmployee(employeeId).ToList();
    }


    public IEnumerable<SaleDTO> GetByStatus(SaleStatus status)
    {
        if (!Enum.IsDefined(status))
            throw new ArgumentOutOfRangeException(nameof(status));

        return _unitOfWork.SaleRepository.GetSalesByStatus(status).ToList();
    }


    public IEnumerable<SaleDTO> GetByDateRange(DateTime from, DateTime to)
    {
        if (from > to)
            throw new ArgumentException("From date cannot be greater than to date.");

        return _unitOfWork.SaleRepository.GetSalesByDateRange(from, to).ToList();
    }


    public decimal GetDailyIncome(DateTime date)
    {
        var from = date.Date;
        var to = from.AddDays(1);

        return _unitOfWork.SaleRepository.GetIncomeByDateRange(from, to);
    }
}