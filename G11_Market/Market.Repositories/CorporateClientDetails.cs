using Market.DTO;
using Market.Services.Interfaces.Repositories;
using System.Data.Common;

namespace Market.Repositories;

internal sealed class CorporateClientDetailsRepository(DbConnection connection)
    : BaseRepository<CorporateClientDetailsDTO>(connection), ICorporateClientDetailsRepository
{
}