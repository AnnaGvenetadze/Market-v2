using Market.Extensions.Attributes;

namespace Market.DTO;

public sealed class CorporateClientDetailsDTO
{
    [IgnoreOnUpdate]
    public int Id { get; set; }
    public string CompanyName { get; set; } 
    public string TaxNumber { get; set; } 
    public string? LegalAddress { get; set; }
    public string? ContactPersonName { get; set; }

    [IgnoreOnUpdate]
    [IgnoreOnInsert]
    public bool IsDeleted { get; set; }

    [IgnoreOnInsert]
    [IgnoreOnUpdate]
    public DateTime CreateDate { get; set; }

    [IgnoreOnInsert]
    [IgnoreOnUpdate]
    public DateTime? UpdateDate { get; set; }
}