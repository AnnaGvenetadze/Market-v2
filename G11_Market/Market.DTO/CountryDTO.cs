using Market.Extensions.Attributes;

namespace Market.DTO;

public class CountryDTO
{
    [IgnoreOnInsert]
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string CountryCode { get; set; } = string.Empty;

    [IgnoreOnInsert]
    [IgnoreOnUpdate]
    public DateTime CreateDate { get; set; }

    [IgnoreOnInsert]
    [IgnoreOnUpdate]
    public DateTime? UpdateDate { get; set; }
}




