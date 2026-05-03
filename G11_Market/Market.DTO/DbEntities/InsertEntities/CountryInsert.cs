using Market.DTO.Interfaces;

namespace Market.DTO.DbEntities.InsertEntities;

public sealed class CountryInsert : IDbInsertEntity
{
    public string Name { get; set; } = null!;
    public string CountryCode { get; set; } = null!;
}