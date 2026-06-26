using Market.Services.Interfaces.Repositories;

namespace Market.Services.Interfaces;

public interface IUnitOfWork : IDisposable
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
    //IStockMovementRepository StockMovementsRepository { get; }
    //ICorporateClientDetailsRepository CorporateClientDetailsRepository { get; }
    //IEmployeeRolesRepository EmployeeRolesRepository { get; }
    //ICityRepository CityRepository { get; }
}