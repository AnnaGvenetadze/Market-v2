using Market.DTO;
using Market.DTO.DbEntities.InsertEntities;
using Market.DTO.DbEntities.UpdateEntities;

namespace Market.Repositories.Interfaces;

public interface ICountryRepository : IBaseRepository<CountryDTO, CountryInsert, CountryUpdate>
{
}
