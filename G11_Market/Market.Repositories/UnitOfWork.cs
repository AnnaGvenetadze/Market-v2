using System.Data.Common;
using Market.Services.Interfaces;
using Market.Services.Interfaces.Repositories;

namespace Market.Repositories;
// todo: Make sure that all units are passing. Add new if needed.
 

internal sealed class UnitOfWork : IUnitOfWork, IDisposable
{
    private bool _disposed;
    private readonly DbConnection _connection;
    private DbTransaction? _transaction;
    private readonly Stack<string> _transactionSavePoints = new();

    private readonly Lazy<CategoryRepository> _categoryRepository;
    private readonly Lazy<EmployeeRepository> _employeeRepository;
    private readonly Lazy<ProductRepository> _productRepository;
    private readonly Lazy<StockMovementRepository> _stockMovementRepository;
    private readonly Lazy<SaleRepository> _saleRepository;
    private readonly Lazy<SaleItemRepository> _saleItemRepository;
    private readonly Lazy<InventoryManagerDetailsRepository> _inventoryManagerDetailsRepository;
    private readonly Lazy<RoleRepository> _roleRepository;
    private readonly Lazy<EmployeeRoleRepository> _employeeRoleRepository;
    private readonly Lazy<CorporateClientDetailsRepository> _corporateClientDetailsRepository;
    private readonly Lazy<CountryRepository> _countryRepository;
    private readonly Lazy<CityRepository> _cityRepository;
    private readonly Lazy<AccountRepository> _accountRepository;
    private readonly Lazy<ClientRepository> _clientRepository;
    private readonly Lazy<AttributeRepository> _attributeRepository;
    private readonly Lazy<ProductAttributeValueRepository> _productAttributeValueRepository;

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
        _stockMovementRepository = new Lazy<StockMovementRepository>(() => new StockMovementRepository(_connection));
        _employeeRoleRepository = new Lazy<EmployeeRoleRepository>(() => new EmployeeRoleRepository(_connection));
        _corporateClientDetailsRepository = new Lazy<CorporateClientDetailsRepository>(() => new CorporateClientDetailsRepository(_connection));
        _cityRepository = new Lazy<CityRepository>(() => new CityRepository(_connection));

        _disposed = false;
    }

    public ICategoryRepository CategoryRepository
        => GetRepository(_categoryRepository);

    public IEmployeeRepository EmployeeRepository
        => GetRepository(_employeeRepository);

    public IProductRepository ProductRepository
        => GetRepository(_productRepository);

    public IStockMovementRepository StockMovementsRepository
        => GetRepository(_stockMovementRepository);

    public ISaleRepository SaleRepository
        => GetRepository(_saleRepository);

    public ISaleItemRepository SaleItemRepository
        => GetRepository(_saleItemRepository);

    public IRoleRepository RoleRepository
        => GetRepository(_roleRepository);

    public IInventoryManagerDetailsRepository InventoryManagerDetailsRepository
        => GetRepository(_inventoryManagerDetailsRepository);

    public ICorporateClientDetailsRepository CorporateClientDetailsRepository
        => GetRepository(_corporateClientDetailsRepository);

    public ICountryRepository CountryRepository
        => GetRepository(_countryRepository);

    public ICityRepository CityRepository
        => GetRepository(_cityRepository);

    public IEmployeeRoleRepository EmployeeRolesRepository
        => GetRepository(_employeeRoleRepository);

    public IAccountRepository AccountRepository
        => GetRepository(_accountRepository);

    public IClientRepository ClientRepository
        => GetRepository(_clientRepository);

    public IAttributeRepository AttributeRepository
        => GetRepository(_attributeRepository);

    public IProductAttributeValueRepository ProductAttributeValueRepository
        => GetRepository(_productAttributeValueRepository);

    public IStockMovementRepository StockMovementRepository
         => GetRepository(_stockMovementRepository);

    public IEmployeeRoleRepository EmployeeRoleRepository
        => GetRepository(_employeeRoleRepository);

    public void BeginTransaction()
    {
        ThrowIfDisposed();
        if (_transaction != null)
            throw new InvalidOperationException("Transaction already started");
        _transaction = _connection.BeginTransaction();
        _transactionSavePoints.Clear();
    }

    public void BeginNestedTransaction()
    {
        ThrowIfDisposed();
        EnsureTransactionExists();
        CreateSavePoint();
    }

    public void Commit()
    {
        EnsureTransactionExists();
        if (_transactionSavePoints.Count > 0)
        {
            _transactionSavePoints.Pop();
        }
        else
        {
            CommitRootTransaction();
        }
    }

    public void Rollback()
    {
        EnsureTransactionExists();
        if (_transactionSavePoints.Count > 0)
        {
            RollbackToSavePoint();
        }
        else
        {
            RollbackToRoot();
        }
    }

    public void RollbackToLastSavePoint()
    {
        EnsureTransactionExists();
        if (_transactionSavePoints.Count == 0)
            throw new InvalidOperationException("No savepoints available");
        RollbackToSavePoint();
    }

    public void CommitRootTransaction()
    {
        EnsureTransactionExists();
        _transaction!.Commit();
        CleanUpTransaction();
    }

    private void CreateSavePoint()
    {
        string savePoint = $"sp_{Random.Shared.Next(0, 1000)}";
        _transaction!.Save(savePoint);
        _transactionSavePoints.Push(savePoint);
    }

    public void RollbackToSavePoint()
    {
        string savePoint = _transactionSavePoints.Pop();
        _transaction!.Rollback(savePoint);
    }

    //public void RollbackToSavePoint(string savePoint)
    //{
    //    EnsureTransactionExists();
    //    if (!_transactionSavePoints.Contains(savePoint))
    //        throw new ArgumentException("Savepoint does not exist", nameof(savePoint));
    //    while (_transactionSavePoints.Count > 0)
    //    {
    //        string sp = _transactionSavePoints.Peek();
    //        if (sp == savePoint)
    //        {
    //            _transaction!.Rollback(sp);
    //            break;
    //        }
    //        _transactionSavePoints.Pop();
    //    }
    //}

    public void RollbackToRoot()
    {
        _transaction!.Rollback();
        CleanUpTransaction();
    }

    private void CleanUpTransaction()
    {
        _transaction?.Dispose();
        _transaction = null;
        _transactionSavePoints.Clear();
    }

    private void EnsureTransactionExists()
    {
        if (_transaction == null)
            throw new InvalidOperationException("No active transaction");
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private T GetRepository<T>(Lazy<T> repository) where T : class
    {
        ThrowIfDisposed();
        return repository.Value;
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            _transaction?.Dispose();
            _transaction = null;
            _transactionSavePoints.Clear();
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