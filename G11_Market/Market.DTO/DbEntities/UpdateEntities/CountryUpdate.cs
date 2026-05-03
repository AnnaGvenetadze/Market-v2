using Market.DTO.Interfaces;

namespace Market.DTO.DbEntities.UpdateEntities;

public sealed class CountryUpdate : IDbUpdateEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string CountryCode { get; set; } = null!;
}