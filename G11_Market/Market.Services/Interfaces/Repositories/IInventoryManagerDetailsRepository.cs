using Market.DTO;

namespace Market.Services.Interfaces.Repositories;

public interface IInventoryManagerDetailsRepository
{
    InventoryManagerDTO? GetByEmployeeId(int employeeId);

    IEnumerable<InventoryManagerDTO> GetAllActive();

    IEnumerable<InventoryManagerDTO> GetManagersWhoCanApproveStockCorrection();

    IEnumerable<InventoryManagerDTO> GetManagersWhoCanApproveNegativeStock();
}