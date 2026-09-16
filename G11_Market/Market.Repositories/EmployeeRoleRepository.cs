using Market.DTO;
using Market.Services.Interfaces.Repositories;
using System.Data.Common;

namespace Market.Repositories;

internal sealed class EmployeeRoleRepository(DbConnection connection)
    : BaseRepository<EmployeeRoleDTO>(connection), IEmployeeRoleRepository
{
}