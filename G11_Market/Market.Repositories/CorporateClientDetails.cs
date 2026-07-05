using Market.DTO;
using Market.Services.Interfaces.Repositories;
using System.Data.Common;

namespace Market.Repositories;

internal class CorporateClientDetailsRepository(DbConnection connection)
    : BaseRepository<CorporateClientDetailsDTO>(connection), ICorporateClientDetailsRepository
{
}