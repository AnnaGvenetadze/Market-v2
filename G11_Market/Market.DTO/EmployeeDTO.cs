using Market.Extensions.Attributes;

namespace Market.DTO;

public sealed class EmployeeDTO
{
    [IgnoreOnInsert]
    [IgnoreOnUpdate]
    public int Id { get; set; }
    public int AccountId { get; set; }
    public int? ManagerEmployeeId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string EmployeeCode { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }
    public string? PhoneNumber { get; set; }
    public string? ContactEmail { get; set; }

    [IgnoreOnInsert]
    [IgnoreOnUpdate]
    public bool IsDeleted { get; set; }

    [IgnoreOnInsert]
    [IgnoreOnUpdate]
    public DateTime CreateDate { get; set; }

    [IgnoreOnInsert]
    [IgnoreOnUpdate]
    public DateTime? UpdateDate { get; set; }
}