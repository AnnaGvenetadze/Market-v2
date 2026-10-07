using Market.DTO;
using Market.DTO.Enums;

public interface ISaleService
{
    int CreateSale();
    SaleDTO? GetById(int saleId);
    void CompleteSale(int saleId);
    void CancelSale(int saleId, string cancelReason);
    void AddItem(int saleId, int productId, int quantity);
    void UpdateItemQuantity(int saleItemId, int quantity);
    void RemoveItem(int saleItemId);
    IEnumerable<SaleDTO> GetByEmployee(int employeeId);
    IEnumerable<SaleDTO> GetByStatus(SaleStatus status);
    IEnumerable<SaleDTO> GetByDateRange(DateTime from, DateTime to);
    decimal GetDailyIncome(DateTime date);
}