using System.Data;
using System.Data.Common;
using Dapper;
using Market.Extensions;

namespace Market.Repositories;

public interface IRepository<T> 
{
    T? GetById(object id);
    int Insert(T entity);
    void Update(T entity);
    void Delete(object id);
    IEnumerable<T> GetAll();
    IEnumerable<T> Search(Predicate<T> predicate);
}

public abstract class BaseRepository<T> : IDisposable
{
    private readonly DbConnection _connection;
    private bool _disposed = false;
    private readonly string _entityName;
    private readonly string _entityPluralName;

    protected BaseRepository(DbConnection connection)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        _entityName = typeof(T).Name;
        _entityPluralName = _entityName.ToPlural();
    }

    public T? GetById(object id)
    {
        ArgumentNullException.ThrowIfNull(id, nameof(id));

        return _connection.QueryFirstOrDefault<T>(
            $"sp_Get{_entityName}ById",
            new { Id = id },
            commandType: CommandType.StoredProcedure);
    }

    public IEnumerable<T> GetAll()
    {
        return _connection.Query<T>(
            $"sp_GetAll{_entityPluralName}", 
            commandType: CommandType.StoredProcedure);
    }

    public int Insert(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));

        var parameters = new DynamicParameters(entity);
        parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
        // todo: take parameters using reflection from entity and pass to sp_Insert{_entityName}

        _connection.Execute(
            $"sp_Insert{_entityName}", 
            parameters, 
            commandType: CommandType.StoredProcedure);

        return parameters.Get<int>("@Id");
    }

    public void Update(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));

        var parameters = new DynamicParameters(entity);
        // todo: take parameters using reflection from entity and pass to sp_Update{_entityName}

        _connection.Execute(
            $"sp_Update{_entityName}",
            parameters,
            commandType: CommandType.StoredProcedure);
    }

    public void Delete(object id)
    {
        ArgumentNullException.ThrowIfNull(id, nameof(id));

        _connection.Execute(
            $"sp_Delete{_entityName}", 
            new { Id = id },
            commandType: CommandType.StoredProcedure);
    }

    public IEnumerable<T> Search(Predicate<T> predicate)
    {
        var allItems = GetAll();
        return allItems.Where(item => predicate(item));
    }

    #region IDisposable Support

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) 
            return;
        if (disposing)
        {
            //if (_connection != null)
            //{
            //    _connection.Dispose();
            //}
        }
        _disposed = true;
    }

    ~BaseRepository()
    {
        Dispose(false);
    }

    #endregion
}
