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
    IInventoryManagerDetailsRepository InventoryManagerDetailsRepository { get; }
    ICountryRepository CountryRepository { get; }
    IAccountRepository AccountRepository { get; }
    IClientRepository ClientRepository { get; }
    IAttributeRepository AttributeRepository { get; }
    IProductAttributeValueRepository ProductAttributeValueRepository { get; }
    IStockMovementRepository StockMovementRepository { get; }
    ICorporateClientDetailsRepository CorporateClientDetailsRepository { get; }
    IEmployeeRoleRepository EmployeeRoleRepository { get; }
    ICityRepository CityRepository { get; }

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