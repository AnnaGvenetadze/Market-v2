using System.Data.Common;
using Market.Services.Interfaces.Repositories;

namespace Market.Repositories;
// todo: Make sure that all units are passing.
// todo: Create interface for UnitOfWork.
// todo: We need to add transaction support to the UnitOfWork class.
// todo: We need to develop factory class for UnitOfWork (not for now).
public sealed class UnitOfWork : IDisposable
{
    private readonly DbConnection _connection;

    private readonly Lazy<CategoryRepository> _categoryRepository;
    private readonly Lazy<EmployeeRepository> _employeeRepository;
    private readonly Lazy<ProductRepository> _productRepository;
    //private readonly Lazy<StockMovementsRepository> _stockMovementsRepository;
    private readonly Lazy<SaleRepository> _saleRepository;
    private readonly Lazy<SaleItemRepository> _saleItemRepository;
    private readonly Lazy<InventoryManagerDetailsRepository> _inventoryManagerDetailsRepository;
    private readonly Lazy<RoleRepository> _roleRepository;
    //private readonly Lazy<EmployeeRolesRepository> _employeesRolesRepository;
    //private readonly Lazy<CorporateClientDetailsRepository> _corporateClientDetailsRepository;
    private readonly Lazy<CountryRepository> _countryRepository;
    //private readonly Lazy<CityRepository> _cityRepository;
    private readonly Lazy<AccountRepository> _accountRepository;
    private readonly Lazy<ClientRepository> _clientRepository;
    private readonly Lazy<AttributeRepository> _attributeRepository;
    private readonly Lazy<ProductAttributeValueRepository> _productAttributeValueRepository;

    private bool _disposed;

    public UnitOfWork(DbConnection connection)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));

        _categoryRepository = new Lazy<CategoryRepository>(() => new CategoryRepository(_connection));
        _employeeRepository = new Lazy<EmployeeRepository>(() => new EmployeeRepository(_connection));
        _productRepository = new Lazy<ProductRepository>(() => new ProductRepository(_connection));
        _saleRepository = new Lazy<SaleRepository>(() => new SaleRepository(_connection));
        _saleItemRepository = new Lazy<SaleItemRepository>(() => new SaleItemRepository(_connection));
        _roleRepository = new Lazy<RoleRepository>(() => new RoleRepository(_connection));
        _inventoryManagerDetailsRepository = new Lazy<InventoryManagerDetailsRepository>(() => new InventoryManagerDetailsRepository(_connection));
        _countryRepository = new Lazy<CountryRepository>(() => new CountryRepository(_connection));
        _accountRepository = new Lazy<AccountRepository>(() => new AccountRepository(_connection));
        _clientRepository = new Lazy<ClientRepository>(() => new ClientRepository(_connection));
        _attributeRepository = new Lazy<AttributeRepository>(() => new AttributeRepository(_connection));
        _productAttributeValueRepository = new Lazy<ProductAttributeValueRepository>(() => new ProductAttributeValueRepository(_connection));
        //_stockMovementsRepository = new Lazy<StockMovementsRepository>(() => new StockMovementsRepository(_connection));
        //_employeeRolesRepository = new Lazy<EmployeeRolesRepository>(() => new EmployeeRolesRepository(_connection));
        //_corporateClientDetailsRepository = new Lazy<CorporateClientDetailsRepository>(() => new CorporateClientDetailsRepository(_connection));
        //_cityRepository = new Lazy<CityRepository>(() => new CityRepository(_connection));

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

    public IEmployeeRepository EmployeeRepository => _employeeRepository.Value;
    public IProductRepository ProductRepository => _productRepository.Value;
    //public IStockMovementRepository StockMovementsRepository => _stockMovementsRepository.Value;
    public ISaleRepository SaleRepository => _saleRepository.Value;
    public ISaleItemRepository SaleItemRepository => _saleItemRepository.Value;
    public IRoleRepository RoleRepository => _roleRepository.Value;
    public IInventoryManagerDetailsRepository InventoryManagerDetailsRepository => _inventoryManagerDetailsRepository.Value;
    //public ICorporateClientDetailsRepository CorporateClientDetailsRepository => _corporateClientDetailsRepository.Value;
    public ICountryRepository CountryRepository => _countryRepository.Value;
    //public ICityRepository CityRepository => _cityRepository.Value;
    //public IEmployeeRolesRepository EmployeeRolesRepository => _employeeRolesRepository.Value;
    public IAccountRepository AccountRepository => _accountRepository.Value;
    public IClientRepository ClientRepository => _clientRepository.Value;
    public IAttributeRepository AttributeRepository => _attributeRepository.Value;
    public IProductAttributeValueRepository ProductAttributeValueRepository => _productAttributeValueRepository.Value;
   
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
