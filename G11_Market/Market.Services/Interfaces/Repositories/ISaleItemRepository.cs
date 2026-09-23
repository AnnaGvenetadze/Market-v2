using Market.DTO;

namespace Market.Services.Interfaces.Repositories;

public interface ISaleItemRepository : IBaseRepository<SaleItemDTO>
{
    public IEnumerable<SaleItemDTO> GetBySaleId(int saleId);
    void UpdateQuantity(int saleItemId, int quantity);
}