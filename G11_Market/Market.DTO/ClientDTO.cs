using Market.Extensions.Attributes;

namespace Market.DTO;

public sealed class ClientDTO
{
    [IgnoreOnInsert]
    public int Id { get; set; }
    public int AccountId { get; set; }
    public int ClientTypeId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
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