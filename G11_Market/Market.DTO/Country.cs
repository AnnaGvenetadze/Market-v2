namespace Market.DTO;

public sealed class Country
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string CountryCode { get; set; } = null!;
    public bool IsDeleted { get; set; }
    public DateTime CreateDate { get; set; } 
    public DateTime? UpdateDate { get; set; }
}