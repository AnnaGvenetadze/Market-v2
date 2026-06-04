using Market.DTO;

namespace Market.Repositories.Interfaces;

public interface ISaleRepository
{
    IEnumerable<SaleDTO> GetSalesByEmployee(int employeeId);
    IEnumerable<SaleDTO> GetCompletedSales();
}
