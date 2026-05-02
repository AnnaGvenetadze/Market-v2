namespace Market.DTO;

public sealed class Country : BaseDTO
{
    public string Name { get; set; } = null!;
    public string CountryCode { get; set; } = null!;
    public bool IsDeleted { get; set; }
}