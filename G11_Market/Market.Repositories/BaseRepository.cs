using System.Data;
using System.Data.Common;
using Dapper;
using Market.Extensions;
using Market.Extensions.Attributes;
using Market.Repositories.Interfaces;

namespace Market.Repositories;

public abstract class BaseRepository<T> : IBaseRepository<T>//, IDisposable
{
    private readonly DbConnection _connection;
    private bool _disposed = false;
    private readonly string _entityName;
    private readonly string _entityPluralName;

    protected BaseRepository(DbConnection connection)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        _entityName = typeof(T).Name[..^3]; // removing "DTO" suffix
        _entityPluralName = _entityName.ToPlural();
    }

    public T GetById(object id)
    {
        ArgumentNullException.ThrowIfNull(id, nameof(id));

        return _connection.QueryFirst<T>(
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
        var parameters = new DynamicParameters();
        var propsToInsert = typeof(T)
            .GetProperties()
            .Where(p => !Attribute.IsDefined(p, typeof(IgnoreOnInsertAttribute)));
        foreach (var prop in propsToInsert)
        {
            parameters.Add(prop.Name, prop.GetValue(entity));
        }
        parameters.Add("Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

        _connection.Execute(
            $"sp_Insert{_entityName}",
            parameters,
            commandType: CommandType.StoredProcedure);

        return parameters.Get<int>("Id");
    }

    public void Update(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));

        var parameters = new DynamicParameters();
        var propsToUpdate = typeof(T)
            .GetProperties()
            .Where(p => !Attribute.IsDefined(p, typeof(IgnoreOnUpdateAttribute)));
        foreach (var prop in propsToUpdate)
        {
            parameters.Add(prop.Name, prop.GetValue(entity));
        }

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

    // todo: we need to translate predicate to sql query. 
    public IEnumerable<T> Search(Predicate<T> predicate)
    {
        var allItems = GetAll();
        return allItems.Where(item => predicate(item));
    }

    // #region IDisposable Support

    //public void Dispose()
    //{
    //    Dispose(true);
    //    GC.SuppressFinalize(this);
    //}

    //private void Dispose(bool disposing)
    //{
    //    if (_disposed)
    //        return;
    //    if (disposing)
    //    {

    //    }
    //    _disposed = true;
    //}

    //~BaseRepository()
    //{
    //    Dispose(false);
    //}

    //#endregion
}