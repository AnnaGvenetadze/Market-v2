using Market.Extensions.Attributes;

namespace Market.DTO;

public sealed class CityDTO 
{
    [IgnoreOnInsert]
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int CountryId { get; set; }

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