using System.Data.Common;
using Market.Services.Interfaces.Repositories;

namespace Market.Repositories;
// todo: First of all we need to fix all syntax errors and make sure that all units are passing.
// todo: Create interface for UnitOfWork.
// todo: We need to add transaction support to the UnitOfWork class.
// todo: We need to develop factory class for UnitOfWork (not for now).
public sealed class UnitOfWork : IDisposable
{
    private readonly DbConnection _connection;

    private readonly Lazy<CategoryRepository> _categoryRepository;
    private readonly Lazy<EmployeeRepository> _employeeRepository;
    private readonly Lazy<ProductRepository> _productsRepository;
    //private readonly Lazy<StockMovementsRepository> _stockMovementsRepository;
    private readonly Lazy<SaleRepository> _salesRepository;
    private readonly Lazy<SaleItemRepository> _saleItemsRepository;
    private readonly Lazy<RoleRepository> _rolesRepository;
    private readonly Lazy<InventoryManagerDetailsRepository> _inventoryManagerDetailsRepository;
    private readonly Lazy<RoleRepository> _employeesRolesRepository;
    //private readonly Lazy<CorporateClientDetailRepository> _corporateClientDetailssRepository;
    private readonly Lazy<CountryRepository> _countryRepository;
    //private readonly Lazy<CityRepository> _cityRepository;
    //private readonly Lazy<AccountsRepository> _accountsRepository;
    private readonly Lazy<ClientRepository> _clientRepository;
    //private readonly Lazy<ClientTypeRepository> _clientTypeRepository;
    private readonly Lazy<AttributeRepository> _attributeRepository;
    private readonly Lazy<ProductAttributeValueRepository> _productAttributeValueRepository;

    private bool _disposed;

    public UnitOfWork(DbConnection connection)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));

        _categoryRepository = new Lazy<CategoryRepository>(() => new CategoryRepository(_connection));
        _employeeRepository = new Lazy<EmployeesRepository>(() => new EmployeesRepository(_connection));
        _productsRepository = new Lazy<ProductsRepository>(() => new ProductsRepository(_connection));
        _stockMovementsRepository = new Lazy<StockMovementsRepository>(() => new StockMovementsRepository(_connection));
        _salesRepository = new Lazy<SalesRepository>(() => new SalesRepository(_connection));
        _saleItemsRepository = new Lazy<SaleItemsRepository>(() => new SaleItemsRepository(_connection));
        _rolesRepository = new Lazy<RolesRepository>(() => new RolesRepository(_connection));
        _inventoryManagerDetailsRepository = new Lazy<InventoryManagerDetailsRepository>(() => new InventoryManagerDetailsRepository(_connection));
        _employeesRolesRepository = new Lazy<EmployeesRolesRepository>(() => new EmployeesRolesRepository(_connection));
        _corporateClientDetailssRepository = new Lazy<CorporateClientDetailssRepository>(() => new CorporateClientDetailssRepository(_connection));
        _countryRepository = new Lazy<CountryRepository>(() => new CountryRepository(_connection));
        _cityRepository = new Lazy<CityRepository>(() => new CityRepository(_connection));
        _accountsRepository = new Lazy<AccountsRepository>(() => new AccountsRepository(_connection));
        _clientRepository = new Lazy<ClientRepository>(() => new ClientRepository(_connection));
        _clientTypeRepository = new Lazy<ClientTypeRepository>(() => new ClientTypeRepository(_connection));
        _attributeRepository = new Lazy<AttributeRepository>(() => new AttributeRepository(_connection));
        _productAttributeValueRepository = new Lazy<ProductAttributeValueRepository>(() => new ProductAttributeValueRepository(_connection));

        _disposed = false;
    }

    public ICategoryRepository CategoryRepository
    {
        get
        {
            // todo: move this to helper method to avoid code duplication.
            ThrowIfDisposed();
            return _categoryRepository.Value;
        }
    }

    public IEmployees EmployeeRepository => _employeeRepository.Value;
    public IProducts ProductsRepository => _productsRepository.Value;
    public IStockMovements StockMovementsRepository => _stockMovementsRepository.Value;
    public ISales SalesRepository => _salesRepository.Value;
    public ISaleItems SaleItemsRepository => _saleItemsRepository.Value;
    public IRoles RolesRepository => _rolesRepository.Value;
    public IInventoryManagerDetails InventoryManagerDetailsRepository => _inventoryManagerDetailsRepository.Value;
    public IEmployeesRoles EmployeesRolesRepository => _employeesRolesRepository.Value;
    public ICorporateClientDetails CorporateClientDetailssRepository => _corporateClientDetailssRepository.Value;
    public ICountry CountryRepository => _countryRepository.Value;
    public ICity CityRepository => _cityRepository.Value;
    public IAccounts AccountsRepository => _accountsRepository.Value;
    public IClient ClientRepository => _clientRepository.Value;
    public IClientType ClientTypeRepository => _clientTypeRepository.Value;
    public IAttribute AttributeRepository => _attributeRepository.Value;
    public IProductAttributeValue ProductAttributeValueRepository => _productAttributeValueRepository.Value;
   
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
            // Dispose managed resources here
        }

        _disposed = true;
    }

    private void ThrowIfDisposed()
    {
        if (_disposed) 
            throw new ObjectDisposedException("UnitOfWork is disposed");
    }

    ~UnitOfWork()
    {
        Dispose(false);
    }
}
