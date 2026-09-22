using Market.DTO;

namespace Market.Services.Interfaces.Repositories;

public interface ICityRepository : IBaseRepository<CityDTO>
{
    CityDTO? GetByNameAndCountryId(string name, int countryId);
    IEnumerable<CityDTO> GetByCountryId(int countryId);
    IEnumerable<CityDTO> GetDeletedCities();
}