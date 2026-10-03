using System.Data.Common;
using Market.Services.Interfaces.Repositories;

namespace Market.Services.Interfaces;

public interface IUnitOfWork
{
    ICategoryRepository CategoryRepository { get; }
    IEmployeeRepository EmployeeRepository { get; }
    IProductRepository ProductRepository { get; }
    ISaleRepository SaleRepository { get; }
    ISaleItemRepository SaleItemRepository { get; }
    IRoleRepository RoleRepository { get; }
    IAccountRepository AccountRepository { get; }
    IAttributeRepository AttributeRepository { get; }
    IStockMovementRepository StockMovementRepository { get; }

    void BeginTransaction();

    void BeginNestedTransaction();

    void Commit();

    void Rollback();

    void RollbackToLastSavePoint();

    void CommitRootTransaction();

    void RollbackToSavePoint();

    void RollbackToSavePoint(string savePoint);

    void RollbackToRoot();
}