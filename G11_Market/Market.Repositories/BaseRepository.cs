using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Reflection;
using Dapper;
using Market.Extensions;
using Market.Extensions.Attributes;
using Market.Services.Interfaces.Repositories;

namespace Market.Repositories;

internal abstract class BaseRepository<T> : IBaseRepository<T>, IDisposable
{
    protected readonly DbConnection _connection;
    private bool _disposed = false;
    private readonly string _entityName;
    private readonly string _entityPluralName;
    private static readonly bool TypeHasIsDeleted = CheckIfTypeHasIsDeleted();

    protected BaseRepository(DbConnection connection)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        _entityName = typeof(T).Name[..^3]; // removing "DTO" suffix
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
        var parameters = new DynamicParameters();
        var propsToInsert = typeof(T)
            .GetProperties()
            .Where(p => !Attribute.IsDefined(p, typeof(IgnoreOnInsertAttribute)));
        
        // თუ არ აქვს [IgnoreOnInsert]
        var idProperty = propsToInsert 
            .FirstOrDefault(p => p.Name == "Id");

        foreach (var prop in propsToInsert)
        {
            if (prop.Name == "Id")
                continue;

            parameters.Add(prop.Name, prop.GetValue(entity));
        }

        // თუ Id INSERT-ში მონაწილეობს, ანუ entity-ს უკვე აქვს Id
        // და ის SQL-ში უნდა გადავცეთ.
        if (idProperty != null)
        {
            parameters.Add(
                "Id",
                idProperty.GetValue(entity),
                dbType: DbType.Int32,
                direction: ParameterDirection.InputOutput);
        }
        else // თვითგენერირებადი id
        {
            parameters.Add(
                "Id",
                dbType: DbType.Int32,
                direction: ParameterDirection.Output);
        }

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

    public void Restore(object id)
    {
        ArgumentNullException.ThrowIfNull(id, nameof(id));
        _connection.Execute(
            $"sp_Restore{_entityName}",
            new { Id = id },
            commandType: CommandType.StoredProcedure);
    }

    public IEnumerable<T> Search(Expression<Func<T, bool>> expression)
    {
        ExpressionTranslator<T> translator = new();
        var (sql, parameters) = translator.Translate(expression);
        bool expressionHasIsDeleted =
               translator.ContainsProperty(expression, "IsDeleted");

        string sqlQuery;
        if (TypeHasIsDeleted && !expressionHasIsDeleted)
        {
            sqlQuery = $"SELECT * FROM {_entityPluralName} WHERE IsDeleted = 0 AND {sql}";
        }
        else
        {
            sqlQuery = $"SELECT * FROM {_entityPluralName} WHERE {sql}";
        }


        return _connection.Query<T>(sqlQuery, parameters);
    }

    private static bool CheckIfTypeHasIsDeleted()
    {
        PropertyInfo? property =
            typeof(T).GetProperty(
                "IsDeleted",
                BindingFlags.Public | BindingFlags.Instance);

        if (property == null || property.PropertyType != typeof(bool))
        {
            return false;
        }

        return true;
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

        }
        _disposed = true;
    }

    ~BaseRepository()
    {
        Dispose(false);
    }

    #endregion
}
