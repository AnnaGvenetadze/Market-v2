using System.Data.Common;
using Market.DTO;
using Market.Repositories.Interfaces;

namespace Market.Repositories;

public sealed class SaleItemRepository(DbConnection connection) : BaseRepository<SaleItemDTO>(connection), ISaleItemRepository
{
}