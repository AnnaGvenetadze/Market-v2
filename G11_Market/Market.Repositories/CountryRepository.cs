using System.Data.Common;
using Market.DTO;
using Market.Repositories.Interfaces;

namespace Market.Repositories;

public sealed class CountryRepository(DbConnection connection) : BaseRepository<CountryDTO>(connection), ICountryRepository
{
}