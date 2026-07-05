using Market.DTO;

namespace Market.Services.Interfaces.Repositories;

public interface ISaleRepository : IBaseRepository<SaleDTO>
{
    IEnumerable<SaleDTO> GetSalesByEmployee(int employeeId);
    IEnumerable<SaleDTO> GetCompletedSales();
    public void Cancel(int id, int employeeId, string cancelReason);
}
