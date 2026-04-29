using System.Data;
using System.Data.Common;
using Dapper;
using Market.DTO;

namespace Market.Repositories;

public sealed class CountryRepository : IDisposable
{
    private readonly DbConnection _connection;

    public CountryRepository(DbConnection connection)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
    }

    public Country? GetById(int id)
    {
        return _connection.QueryFirstOrDefault<Country>(
            "sp_GetCountryById",
            new { Id = id },
            commandType: CommandType.StoredProcedure);
    }

    public IEnumerable<Country> GetAll()
    {
        return _connection.Query<Country>(
            "sp_GetAllCountries",
            commandType: CommandType.StoredProcedure);
    }

    public IEnumerable<Country> Search(Predicate<Country> predicate)
    {
        throw new NotImplementedException();
    }

    public int Insert(Country country)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@Name", country.Name);
        parameters.Add("@CountryCode", country.CountryCode);
        parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

        _connection.Execute(
            "sp_InsertCountry",
            parameters,
            commandType: CommandType.StoredProcedure);

        return parameters.Get<int>("@Id");
    }

    public int Update(Country country)
    {
        throw new NotImplementedException();
    }

    public int Delete(int id)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        //throw new NotImplementedException();
    }
}