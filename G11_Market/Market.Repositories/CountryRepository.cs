using System.Data.Common;
using Market.DTO;

namespace Market.Repositories;

public sealed class CountryRepository(DbConnection connection, string tableName) : BaseRepository<Country>(connection, tableName)
{
}