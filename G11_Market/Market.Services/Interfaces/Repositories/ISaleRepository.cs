using Market.DTO;
using Market.DTO.Enums;

namespace Market.Services.Interfaces.Repositories;

public interface ISaleRepository : IBaseRepository<SaleDTO>
{
    IEnumerable<SaleDTO> GetSalesByEmployee(int employeeId);
    IEnumerable<SaleDTO> GetSalesByStatus(SaleStatus status);
    IEnumerable<SaleDTO> GetSalesByDateRange(DateTime dateFrom, DateTime dateTo);
    decimal GetIncomeByDateRange(DateTime dateFrom, DateTime dateTo);
    void CompleteSale(int saleId, int employeeId);
    public void CancelSale(int id, int employeeId, string cancelReason);
}
