using Market.DTO;
using Market.Services.Interfaces.Repositories;
using System.Data.Common;

namespace Market.Repositories;

public class StockMovementRepository(DbConnection connection)
    : BaseRepository<StockMovementDTO>(connection), IStockMovementRepository
{
}