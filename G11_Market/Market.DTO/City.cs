namespace Market.DTO;

public sealed class City : BaseDTO
{
    public string Name { get; set; } = null!;
    public int CountryId { get; set; }
    public bool IsDeleted { get; set; }
}