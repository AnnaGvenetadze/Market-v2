using System.Data.Common;
using Dapper;
using Market.DTO;
using Market.Services.Interfaces.Repositories;

namespace Market.Repositories;

internal sealed class RoleRepository(DbConnection connection)
    : BaseRepository<RoleDTO>(connection), IRoleRepository
{
    public RoleDTO? GetByName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return Search(role =>
                role.Name == name &&
                role.IsDeleted == false)
            .FirstOrDefault();
    }
    public async Task<IReadOnlyList<RoleDTO>> GetByAccountIdAsync(
        int accountId,
        CancellationToken cancellationToken = default)
    {
        if (accountId <= 0)
            throw new ArgumentOutOfRangeException(nameof(accountId));

        const string sql = """
        SELECT DISTINCT
            r.Id,
            r.Name,
            r.Description,
            r.IsDeleted,
            r.CreateDate,
            r.UpdateDate
        FROM dbo.Roles AS r
        INNER JOIN dbo.EmployeeRoles AS er
            ON er.RoleId = r.Id
        INNER JOIN dbo.Employees AS e
            ON e.Id = er.EmployeeId
        INNER JOIN dbo.Accounts AS a
            ON a.Id = e.AccountId
        WHERE a.Id = @AccountId
          AND a.IsDeleted = 0
          AND a.IsActive = 1
          AND e.IsDeleted = 0
          AND r.IsDeleted = 0
        ORDER BY r.Name;
        """;

        var command = new CommandDefinition(
            commandText: sql,
            parameters: new { AccountId = accountId },
            cancellationToken: cancellationToken);

        var roles = await _connection.QueryAsync<RoleDTO>(command);

        return roles.ToList();
    }
}