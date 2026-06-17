using System.Data.Common;
using Market.Repositories.Interfaces;

namespace Market.Repositories;

public sealed class UnitOfWork
{
    private readonly DbConnection _connection;
    private readonly Lazy<CategoryRepository> _categoryRepository;
    private CountryRepository? _countryRepository;
    private EmployeeRepository? _employeeRepository;

    public UnitOfWork(DbConnection connection)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        _categoryRepository = new Lazy<CategoryRepository>(() => new CategoryRepository(_connection));
    }

    public ICategoryRepository CategoryRepository => _categoryRepository.Value;

    public ICountryRepository CountryRepository => _countryRepository ??= new CountryRepository(_connection);

    public IEmployeeRepository EmployeeRepository
    {
        get
        {
            if (_employeeRepository == null)
            {
                _employeeRepository = new EmployeeRepository(_connection);
            }
            return _employeeRepository;
        }
    }
}