using Market.DTO;
using Market.Repositories.Interfaces;
using System.Data.Common;


namespace Market.Repositories
{
    public sealed class ProductRepository(DbConnection connection)
        : BaseRepository<ProductDTO>(connection), IProductRepository
    {
    }
}
