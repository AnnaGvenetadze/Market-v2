using System.Data;
using System.Data.Common;
using Dapper;
using Market.Extensions;
using Market.DTO.Interfaces;
using Market.Repositories.Interfaces;

namespace Market.Repositories.Implementations;

public abstract class BaseRepository<TDto,TInsert,TUpdate> : IBaseRepository<TDto, TInsert, TUpdate>, IDisposable
    where TDto : IDto
    where TInsert : IDbInsertEntity
    where TUpdate : IDbUpdateEntity
{
    private readonly DbConnection _connection;
    private bool _disposed = false;
    private readonly string _entityName;
    private readonly string _entityPluralName;
    private bool _keepConnectionOpen = true;

    protected BaseRepository(DbConnection connection, bool keepConnectionOpen)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        _entityName = typeof(TDto).Name;
        _entityPluralName = _entityName.ToPlural();
        _keepConnectionOpen = keepConnectionOpen;
    }

    public TDto? GetById(object id)
    {
        ArgumentNullException.ThrowIfNull(id, nameof(id));

        return _connection.QueryFirstOrDefault<TDto>(
            $"sp_Get{_entityName}ById",
            new { Id = id },
            commandType: CommandType.StoredProcedure);
    }

    public IEnumerable<TDto> GetAll()
    {
        return _connection.Query<TDto>(
            $"sp_GetAll{_entityPluralName}", 
            commandType: CommandType.StoredProcedure);
    }

    public int Insert(TInsert entity)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));

        var parameters = new DynamicParameters(entity);
        parameters.Add("@IdOut", dbType: DbType.Int32, direction: ParameterDirection.Output);

        _connection.Execute(
            $"sp_Insert{_entityName}", 
            parameters, 
            commandType: CommandType.StoredProcedure);

        return parameters.Get<int>("@IdOut");
    }

    public void Update(TUpdate entity) 
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));

        _connection.Execute(
            $"sp_Update{_entityName}",
            entity,
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

    public IEnumerable<TDto> Search(Predicate<TDto> predicate)
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
            if (!_keepConnectionOpen)
            {
                _connection.Dispose();
            }
        }
        _disposed = true;
    }

    ~BaseRepository()
    {
        Dispose(false);
    }

    #endregion
}
