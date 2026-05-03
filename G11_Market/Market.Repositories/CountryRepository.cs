using System.Data.Common;
using Market.DTO;

namespace Market.Repositories;

public interface ICountryRepository : IRepository<Country>
{

}

public sealed class CountryRepository : BaseRepository<Country>, ICountryRepository
{
    public CountryRepository(DbConnection connection) : base(connection)
    {

    }
}