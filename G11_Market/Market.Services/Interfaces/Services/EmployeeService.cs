using Market.DTO;
using Market.Extensions;
using Serilog;

namespace Market.Services.Interfaces.Services;

public class EmployeeService : IEmployeeService
{
    private readonly ILogger _logger;
    private readonly IUnitOfWork _unitOfWork;

    public EmployeeService(IUnitOfWork unitOfWork, ILogger logger)
    {
        _unitOfWork = unitOfWork
            ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));
    }

    public void ChangeRole(int employeeId, int roleId)
    {
        if (employeeId <= 0)
            throw new ArgumentOutOfRangeException(nameof(employeeId));
        if (roleId <= 0)
            throw new ArgumentOutOfRangeException(nameof(roleId));

        var currentRoles = _unitOfWork.EmployeeRepository.GetRoles(employeeId);
        foreach (var role in currentRoles)
        {
            _unitOfWork.EmployeeRepository.UnassignRole(new EmployeeRoleDTO
            {
                EmployeeId = employeeId,
                RoleId = role.Id
            });
        }

        _unitOfWork.EmployeeRepository.AssignRole(new EmployeeRoleDTO
        {
            EmployeeId = employeeId,
            RoleId = roleId
        });
    }

    public EmployeeDTO CreateEmployee(CreateEmployeeDTO dto)
    {
        PasswordHasher.HashPassword(dto.Password);
        ArgumentNullException.ThrowIfNull(dto);
        ArgumentException.ThrowIfNullOrWhiteSpace(dto.FirstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(dto.LastName);
        ArgumentException.ThrowIfNullOrWhiteSpace(dto.Username);
        ArgumentException.ThrowIfNullOrWhiteSpace(dto.Password);
        string hashedPassword = PasswordHasher.HashPassword(dto.Password);
        if (dto.RoleId <= 0)
            throw new ArgumentOutOfRangeException(nameof(dto.RoleId));

        _unitOfWork.BeginTransaction();
        try
        {
            int accountId = _unitOfWork.AccountRepository.Insert(new AccountDTO
            {
                Username = dto.Username,
                PasswordHash = hashedPassword,
            });

            string employeeCode = $"EMP-{Guid.NewGuid().ToString()[..8].ToUpper()}";
            var employeeDto = new EmployeeDTO
            {
                AccountId = accountId,
                ManagerEmployeeId = dto.ManagerEmployeeId,
                EmployeeCode = employeeCode,
                HireDate = DateTime.UtcNow,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Username,
                PhoneNumber = string.Empty
            };

            int employeeId = _unitOfWork.EmployeeRepository.Insert(employeeDto);
            _unitOfWork.EmployeeRepository.AssignRole(new EmployeeRoleDTO
            {
                EmployeeId = employeeId,
                RoleId = dto.RoleId
            });
            _unitOfWork.Commit();
            return _unitOfWork.EmployeeRepository.GetById(employeeId)
                ?? throw new InvalidOperationException($"Failed to retrieve employee after creation (ID: {employeeId}).");
        }
        catch
        {
            _unitOfWork.Rollback();
            throw;
        }
    }

    public void DeactivateEmployee(int employeeId)
    {
        if (employeeId <= 0)
            throw new ArgumentOutOfRangeException(nameof(employeeId));

        _unitOfWork.BeginTransaction();
        try
        {
            var employee = _unitOfWork.EmployeeRepository.GetById(employeeId);
            if (employee == null)
                throw new KeyNotFoundException($"Employee with ID {employeeId} was not found.");
            _unitOfWork.EmployeeRepository.Delete(employeeId);
            _unitOfWork.AccountRepository.Delete(employee.AccountId);
            _unitOfWork.Commit();
        }
        catch
        {
            _unitOfWork.Rollback();
            throw;
        }
    }

    public IEnumerable<EmployeeDTO> GetAll() => _unitOfWork.EmployeeRepository.GetAll();

    public EmployeeDTO? GetById(int employeeId)
    {
        if (employeeId <= 0)
            throw new ArgumentOutOfRangeException(nameof(employeeId));

        return _unitOfWork.EmployeeRepository.GetById(employeeId);
    }

    public void ReactivateEmployee(int employeeId)
    {
        if (employeeId <= 0)
            throw new ArgumentOutOfRangeException(nameof(employeeId));

        _unitOfWork.BeginTransaction();
        try
        {
            var employee = _unitOfWork.EmployeeRepository.GetById(employeeId);
            if (employee == null)
                throw new KeyNotFoundException($"Employee with ID {employeeId} was not found.");
            _unitOfWork.EmployeeRepository.Restore(employeeId);
            _unitOfWork.AccountRepository.Restore(employee.AccountId);
            _unitOfWork.Commit();
        }
        catch
        {
            _unitOfWork.Rollback();
            throw;
        }
    }

    public void ResetPassword(int employeeId, string newPassword)
    {
        if (employeeId <= 0)
            throw new ArgumentOutOfRangeException(nameof(employeeId));

        ArgumentException.ThrowIfNullOrWhiteSpace(newPassword);
        var employee = _unitOfWork.EmployeeRepository.GetById(employeeId);
        if (employee == null || employee.IsDeleted)
            throw new KeyNotFoundException($"Employee with ID {employeeId} was not found or is inactive.");

        string passwordHash = PasswordHasher.HashPassword(newPassword);

        _unitOfWork.BeginTransaction();
        try
        {
            _unitOfWork.AccountRepository.ResetPassword(employee.AccountId, passwordHash);
            _unitOfWork.Commit();
        }
        catch
        {
            _unitOfWork.Rollback();
            throw;
        }
    }
    public void UpdateAccount(AccountDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (dto.Id <= 0)
            throw new ArgumentOutOfRangeException(nameof(dto.Id));

        _unitOfWork.BeginTransaction();
        try
        {
            var existingAccount = _unitOfWork.AccountRepository.GetById(dto.Id);
            if (existingAccount == null || existingAccount.IsDeleted)
                throw new KeyNotFoundException($"Account with ID {dto.Id} was not found or is deleted.");
            _unitOfWork.AccountRepository.Update(dto);
            _unitOfWork.Commit();
        }
        catch
        {
            _unitOfWork.Rollback();
            throw;
        }
    }

    public void UpdateEmployee(EmployeeDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (dto.Id <= 0)
            throw new ArgumentOutOfRangeException(nameof(dto.Id));

        ArgumentException.ThrowIfNullOrWhiteSpace(dto.FirstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(dto.LastName);

        _unitOfWork.BeginTransaction();
        try
        {
            var existingEmployee = _unitOfWork.EmployeeRepository.GetById(dto.Id);
            if (existingEmployee == null || existingEmployee.IsDeleted)
                throw new KeyNotFoundException($"Employee with ID {dto.Id} was not found or is deleted.");
            _unitOfWork.EmployeeRepository.Update(dto);
            _unitOfWork.Commit();
        }
        catch
        {
            _unitOfWork.Rollback();
            throw;
        }
    }
}