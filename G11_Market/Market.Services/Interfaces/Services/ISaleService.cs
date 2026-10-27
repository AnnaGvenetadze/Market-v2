using Market.DTO;
using Market.DTO.Enums;
using Market.Services.Attributes;
using Market.Services.Enums;

namespace Market.Services.Interfaces.Services;

public interface ISaleService
{
    [RequireRole(Role.Cashier, Role.Administrator)]
    int CreateSale();

    [RequireRole(Role.Cashier, Role.Administrator)]
    SaleDTO? GetById(int saleId);

    [RequireRole(Role.Cashier, Role.Administrator)]
    void CompleteSale(int saleId);

    [RequireRole(Role.Administrator)]
    void CancelSale(int saleId, string cancelReason);

    [RequireRole(Role.Cashier, Role.Administrator)]
    void AddItem(int saleId, int productId, int quantity);

    [RequireRole(Role.Cashier, Role.Administrator)]
    void UpdateItemQuantity(int saleItemId, int quantity);

    [RequireRole(Role.Cashier, Role.Administrator)]
    void RemoveItem(int saleItemId);

    [RequireRole(Role.Administrator)]
    IEnumerable<SaleDTO> GetByEmployee(int employeeId);

    [RequireRole(Role.Administrator)]
    IEnumerable<SaleDTO> GetByStatus(SaleStatus status);

    [RequireRole(Role.Administrator)]
    IEnumerable<SaleDTO> GetByDateRange(DateTime from, DateTime to);

    [RequireRole(Role.Administrator)]
    decimal GetDailyIncome(DateTime date);
}