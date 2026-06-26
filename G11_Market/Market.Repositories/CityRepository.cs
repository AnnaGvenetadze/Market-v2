using Market.DTO;
using Market.Services.Interfaces.Repositories;
using System.Data.Common;

namespace Market.Repositories;

public class CityRepository(DbConnection connection)
    : BaseRepository<CityDTO>(connection), ICityRepository
{
}