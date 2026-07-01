using System.Data;
using System.Data.Common;
using Dapper;
using Market.DTO;
using Market.Services.Interfaces.Repositories;

namespace Market.Repositories;

public sealed class InventoryManagerDetailsRepository(DbConnection connection)
    : BaseRepository<InventoryManagerDetailDTO>(connection), IInventoryManagerDetailsRepository
{
    private readonly DbConnection _connection = connection;

    public new int Insert(InventoryManagerDetailDTO entity)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));

        _connection.Execute(
            "sp_InsertInventoryManager",
            new
            {
                entity.Id, // of exsisting account id
                entity.StockAdjustmentLimit,
                entity.CanApproveStockCorrection,
                entity.CanApproveNegativeStock
            },
            commandType: CommandType.StoredProcedure);

        return entity.Id;
    }

    public InventoryManagerDetailDTO? GetByEmployeeId(int employeeId)
        => Search(manager => manager.Id == employeeId && manager.IsDeleted == false).FirstOrDefault();

    public IEnumerable<InventoryManagerDetailDTO> GetAllActive()
        => Search(manager => manager.IsDeleted == false);

    public IEnumerable<InventoryManagerDetailDTO> GetManagersWhoCanApproveStockCorrection()
        => Search(manager => manager.CanApproveStockCorrection == true && manager.IsDeleted == false);

    public IEnumerable<InventoryManagerDetailDTO> GetManagersWhoCanApproveNegativeStock()
        => Search(manager => manager.CanApproveNegativeStock == true && manager.IsDeleted == false);
}