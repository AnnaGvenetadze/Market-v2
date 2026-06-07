namespace Market.Tests.Helpers;

using Market.DTO;

public static class EmployeeTestDataFactory
{
    public static EmployeeDTO CreateEmployee(
        int accountId,
        string? employeeCode = null,
        string? phoneNumber = null,
        string? contactEmail = null,
        string firstName = "Test",
        string lastName = "Employee",
        int? managerEmployeeId = null,
        DateTime? hireDate = null)
    {
        return new EmployeeDTO
        {
            AccountId = accountId,
            ManagerEmployeeId = managerEmployeeId,
            FirstName = firstName,
            LastName = lastName,
            EmployeeCode = employeeCode ?? "EMP".AddGuid(),
            PhoneNumber = phoneNumber ?? TestDataHelper.CreatePhoneNumber(),
            ContactEmail = contactEmail ?? "employee".AddGuid() + "@gmail.com",
            HireDate = hireDate ?? DateTime.Today,
            IsDeleted = false
        };
    }
}