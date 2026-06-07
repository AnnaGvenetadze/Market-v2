using System.Data;
using System.Data.Common;
using Dapper;
using Market.DTO;
using Market.Repositories.Interfaces;

namespace Market.Repositories;

public sealed class InventoryManagerDetailsRepository(DbConnection connection)
    : BaseRepository<InventoryManagerDTO>(connection), IInventoryManagerDetailsRepository
{
    private readonly DbConnection _connection = connection;

    public new int Insert(InventoryManagerDTO entity)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));

        _connection.Execute(
            "sp_InsertInventoryManager",
            new
            {
                entity.Id,
                entity.StockAdjustmentLimit,
                entity.CanApproveStockCorrection,
                entity.CanApproveNegativeStock
            },
            commandType: CommandType.StoredProcedure);

        return entity.Id;
    }

    public InventoryManagerDTO? GetByEmployeeId(int employeeId)
        => Search(manager => manager.Id == employeeId && !manager.IsDeleted).FirstOrDefault();

    public IEnumerable<InventoryManagerDTO> GetAllActive()
        => Search(manager => !manager.IsDeleted);

    public IEnumerable<InventoryManagerDTO> GetManagersWhoCanApproveStockCorrection()
        => Search(manager => manager.CanApproveStockCorrection && !manager.IsDeleted);

    public IEnumerable<InventoryManagerDTO> GetManagersWhoCanApproveNegativeStock()
        => Search(manager => manager.CanApproveNegativeStock && !manager.IsDeleted);
}