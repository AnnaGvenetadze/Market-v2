using Market.DTO.DbEntities.InsertEntities;
using Market.DTO.DbEntities.UpdateEntities;
using Market.DTO.DTOs;

namespace Market.Repositories.Interfaces;

public interface ICountryRepository : IBaseRepository<CountryDTO, CountryInsert, CountryUpdate>
{
}
