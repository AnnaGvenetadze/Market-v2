using Market.Extensions.Attributes;

namespace Market.DTO;

public sealed class AccountDTO
{
    [IgnoreOnInsert]
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string Email { get; set; } = null!;
    public byte AccountType { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;

    [IgnoreOnUpdate]
    public string Hwid { get; set; } = string.Empty;
    public int FailedLoginAttempts { get; set; }

    public DateTime? LockoutTime { get; set; }

    [IgnoreOnInsert]
    [IgnoreOnUpdate]
    public bool IsDeleted { get; set; }

    [IgnoreOnUpdate]
    [IgnoreOnInsert]
    public DateTime CreateDate { get; set; }

    [IgnoreOnUpdate]
    [IgnoreOnInsert]
    public DateTime? UpdateDate { get; set; }
}