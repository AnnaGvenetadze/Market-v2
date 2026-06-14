using System.Data.Common;
using Market.Repositories.Interfaces;

namespace Market.Repositories;

public sealed class UnitOfWork
{
    private readonly DbConnection _connection;
    private readonly CategoryRepository _categoryRepository;
    private readonly CountryRepository _countryRepository;
    private EmployeeRepository? _employeeRepository;

    public UnitOfWork(DbConnection connection)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        _categoryRepository = new CategoryRepository(_connection);
        _countryRepository = new CountryRepository(_connection);
    }

    public ICategoryRepository CategoryRepository => _categoryRepository;
    public ICountryRepository CountryRepository => _countryRepository;

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