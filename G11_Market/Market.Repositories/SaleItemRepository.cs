using Dapper;
using Market.DTO;
using Market.Services.Interfaces.Repositories;
using System.Data;
using System.Data.Common;

namespace Market.Repositories;

internal sealed class SaleItemRepository(DbConnection connection)
    : BaseRepository<SaleItemDTO>(connection), ISaleItemRepository
{
    public IEnumerable<SaleItemDTO> GetBySaleId(int saleId)
        => Search(item => item.SaleId == saleId);

    public void UpdateQuantity(int saleItemId, int quantity)
    {
        _connection.Execute(
            "sp_UpdateSaleItemQuantity",
            new
            {
                Id = saleItemId,
                Quantity = quantity
            },
            commandType: CommandType.StoredProcedure);
    }
}