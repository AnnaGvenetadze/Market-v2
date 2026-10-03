using Market.Extensions.Attributes;

public sealed class EmployeeDTO
{
    [IgnoreOnInsert]
    public int Id { get; set; }

    public int AccountId { get; set; }
    public int? ManagerEmployeeId { get; set; }
    public string EmployeeCode { get; set; } = null!;
    public DateTime HireDate { get; set; }

    [IgnoreOnInsert]
    [IgnoreOnUpdate]
    public bool IsDeleted { get; set; }

    [IgnoreOnInsert]
    [IgnoreOnUpdate]
    public DateTime CreateDate { get; set; }

    [IgnoreOnInsert]
    [IgnoreOnUpdate]
    public DateTime? UpdateDate { get; set; }

    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
}