using Dapper;
using Market.DTO;
using Market.Services.Interfaces.Repositories;
using System.Data;
using System.Data.Common;

namespace Market.Repositories;

internal sealed class SaleItemRepository(DbConnection connection, Func<DbTransaction?> transaction) : BaseRepository<SaleItemDTO>(connection, transaction), ISaleItemRepository
{
    public IEnumerable<SaleItemDTO> GetBySaleId(int saleId)
    {
        if (saleId <= 0)
            throw new ArgumentOutOfRangeException(nameof(saleId));

        return Search(item => item.SaleId == saleId);
    }

    public void UpdateQuantity(int saleItemId, int quantity)
    {
        if (saleItemId <= 0)
            throw new ArgumentOutOfRangeException(nameof(saleItemId));
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity));

        _connection.Execute(
            "sp_UpdateSaleItemQuantity",
            new
            {
                Id = saleItemId,
                Quantity = quantity
            },
            transaction: _transaction(),
            commandType: CommandType.StoredProcedure);
    }
}