using System.Data;
using System.Data.Common;
using Dapper;
using Market.DTO;

namespace Market.Repositories;

public abstract class BaseRepository<T> : IDisposable where T : BaseDTO
{
    protected readonly DbConnection _connection;
    protected readonly string _tableName;
    private bool _disposed = false;

    protected BaseRepository(DbConnection connection, string tableName)
    {
        _connection = connection;
        _tableName = tableName;
    }
    protected T? GetById(int id)
    {
        return _connection.QueryFirstOrDefault<T>($"sp_Get{_tableName}ById", new { Id = id }, commandType: CommandType.StoredProcedure);
    }

    protected IEnumerable<T> GetAll()
    {
        return _connection.Query<T>($"sp_GetAll{_tableName}", commandType: CommandType.StoredProcedure);
    }

    protected int Insert(T tableModel)
    {
        var parameters = new DynamicParameters(tableModel);
        parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
        _connection.Execute($"sp_Insert{_tableName}", parameters, commandType: CommandType.StoredProcedure);
        return parameters.Get<int>("@Id");
    }

    protected int Delete(int id)
    {
        return _connection.Execute($"sp_Delete{_tableName}", new { Id = id }, commandType: CommandType.StoredProcedure);
    }

    protected int Update(T tableModel)
    {
        return _connection.Execute($"sp_Update{_tableName}", tableModel, commandType: CommandType.StoredProcedure);
    }

    public IEnumerable<T> Search(Predicate<T> predicate)
    {
        var allItems = GetAll();
        return allItems.Where(item => predicate(item));
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                if (_connection != null)
                {
                    _connection.Dispose();
                }
            }
            _disposed = true;
        }
    }

    ~BaseRepository()
    {
        Dispose(false);
    }
}
