using Market.DTO;
using Market.Services.Interfaces.Repositories;
using System.Data.Common;


namespace Market.Repositories
{
    public sealed class ProductRepository(DbConnection connection)
        : BaseRepository<ProductDTO>(connection), IProductRepository
    {
    }
}
