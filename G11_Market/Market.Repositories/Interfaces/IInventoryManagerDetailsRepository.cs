using Market.DTO;

namespace Market.Repositories.Interfaces;

public interface IInventoryManagerDetailsRepository
{
    InventoryManagerDTO? GetByEmployeeId(int employeeId);

    IEnumerable<InventoryManagerDTO> GetAllActive();

    IEnumerable<InventoryManagerDTO> GetManagersWhoCanApproveStockCorrection();

    IEnumerable<InventoryManagerDTO> GetManagersWhoCanApproveNegativeStock();
}