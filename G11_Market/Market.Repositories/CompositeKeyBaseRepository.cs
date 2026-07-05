using System.Data;
using System.Data.Common;
using Dapper;
using Market.Extensions;
using Market.Extensions.Attributes;
using Market.Services.Interfaces.Repositories;

namespace Market.Repositories;

// TODO: აღარ უნდა? რისთვის იყო
public abstract class CompositeKeyBaseRepository<T> : ICompositeKeyBaseRepository<T>
{
    private readonly DbConnection _connection;

    private readonly string _entityName;
    private readonly string _entityPluralName;

    protected CompositeKeyBaseRepository(DbConnection connection)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));

        _entityName = typeof(T).Name[..^3];
        _entityPluralName = _entityName.ToPlural();
    }

    protected abstract string FirstKeyName { get; }
    protected abstract string SecondKeyName { get; }

    public IEnumerable<T> GetById(object firstId)
    {
        ArgumentNullException.ThrowIfNull(firstId, nameof(firstId));

        var parameters = new DynamicParameters();
        parameters.Add(FirstKeyName, firstId);

        return _connection.Query<T>(
            $"dbo.sp_Get{_entityName}ById",
            parameters,
            commandType: CommandType.StoredProcedure);
    }


    public IEnumerable<T> GetAll()
    {
        return _connection.Query<T>(
            $"dbo.sp_GetAll{_entityPluralName}",
            commandType: CommandType.StoredProcedure);
    }


    public void Insert(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));

        var parameters = new DynamicParameters();

        var propertiesToInsert = typeof(T)
            .GetProperties()
            .Where(
                property => !Attribute.IsDefined(
                    property, typeof(IgnoreOnInsertAttribute))
            );

        foreach (var property in propertiesToInsert)
        {
            parameters.Add(property.Name, property.GetValue(entity));
        }

        _connection.Execute(
            $"dbo.sp_Insert{_entityName}",
            parameters,
            commandType: CommandType.StoredProcedure);
    }


    public void Update(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));

        var parameters = new DynamicParameters();

        var propertiesToUpdate = typeof(T)
            .GetProperties()
            .Where(property => !Attribute.IsDefined(property, typeof(IgnoreOnUpdateAttribute)));

        foreach (var property in propertiesToUpdate)
        {
            parameters.Add(property.Name, property.GetValue(entity));
        }

        _connection.Execute(
            $"dbo.sp_Update{_entityName}",
            parameters,
            commandType: CommandType.StoredProcedure);
    }


    public void Delete(object firstId, object secondId)
    {
        ArgumentNullException.ThrowIfNull(firstId, nameof(firstId));
        ArgumentNullException.ThrowIfNull(secondId, nameof(secondId));

        var parameters = new DynamicParameters();
        parameters.Add(FirstKeyName, firstId);
        parameters.Add(SecondKeyName, secondId);

        _connection.Execute(
            $"dbo.sp_Delete{_entityName}",
            parameters,
            commandType: CommandType.StoredProcedure);
    }
}