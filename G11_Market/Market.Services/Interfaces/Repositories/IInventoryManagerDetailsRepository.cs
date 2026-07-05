using Market.DTO;

namespace Market.Services.Interfaces.Repositories;

public interface IInventoryManagerDetailsRepository : IBaseRepository<InventoryManagerDetailDTO>
{
    InventoryManagerDetailDTO? GetByEmployeeId(int employeeId);

    IEnumerable<InventoryManagerDetailDTO> GetAllActive();

    IEnumerable<InventoryManagerDetailDTO> GetManagersWhoCanApproveStockCorrection();

    IEnumerable<InventoryManagerDetailDTO> GetManagersWhoCanApproveNegativeStock();
}