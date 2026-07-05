using System.Data.Common;
using Market.DTO;
using Market.Services.Interfaces.Repositories;

namespace Market.Repositories;

internal sealed class SaleItemRepository(DbConnection connection)
    : BaseRepository<SaleItemDTO>(connection), ISaleItemRepository
{
    public IEnumerable<SaleItemDTO> GetBySaleId(int saleId)
        => Search(item => item.SaleId == saleId);

    public IEnumerable<SaleItemDTO> GetByProductId(int productId)
        => Search(item => item.ProductId == productId);
}