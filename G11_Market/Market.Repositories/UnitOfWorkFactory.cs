using System.Data.Common;
using Market.Services.Interfaces;

namespace Market.Repositories;

public static class UnitOfWorkFactory
{
    public static IUnitOfWork Create(DbConnection connection)
    {
        return new UnitOfWork(connection);
    }
}