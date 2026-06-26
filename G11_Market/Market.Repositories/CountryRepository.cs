using System.Data.Common;
using Market.DTO;
using Market.Services.Interfaces.Repositories;

namespace Market.Repositories;

public sealed class CountryRepository(DbConnection connection) : BaseRepository<CountryDTO>(connection), ICountryRepository
{
}