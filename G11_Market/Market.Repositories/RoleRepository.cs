using System.Data;
using System.Data.Common;
using Dapper;
using Market.DTO;
using Market.Services.Interfaces.Repositories;

namespace Market.Repositories;

internal sealed class RoleRepository(DbConnection connection, Func<DbTransaction?> transaction) : BaseRepository<RoleDTO>(connection, transaction), IRoleRepository
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

        const string sql = "dbo.sp_GetRolesByAccountId";

        var command = new CommandDefinition(
            commandText: sql,
            parameters: new { AccountId = accountId },
            transaction: _transaction(),
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        var roles = await _connection.QueryAsync<RoleDTO>(command);

        return roles.ToList();
    }
}