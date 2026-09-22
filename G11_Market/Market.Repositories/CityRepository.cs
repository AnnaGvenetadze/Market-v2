using Market.DTO;
using Market.Services.Interfaces.Repositories;
using System.Data.Common;

namespace Market.Repositories;

internal sealed class CityRepository(DbConnection connection)
    : BaseRepository<CityDTO>(connection), ICityRepository
{
    public CityDTO? GetByNameAndCountryId(string name, int countryId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(countryId);

        var trimmedName = name.Trim();

        return Search(city =>
                city.CountryId == countryId &&
                city.Name == trimmedName)
            .FirstOrDefault();
    }

    public IEnumerable<CityDTO> GetByCountryId(int countryId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(countryId);
        return Search(city => city.CountryId == countryId);
    }

    public IEnumerable<CityDTO> GetDeletedCities()
    {
        return Search(city => city.IsDeleted == true);
    }
}