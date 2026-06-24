using Market.DTO;

namespace Market.Services.Interfaces.Repositories;

public interface ISaleRepository
{
    IEnumerable<SaleDTO> GetSalesByEmployee(int employeeId);
    IEnumerable<SaleDTO> GetCompletedSales();
}
