using Market.DTO;
using Market.Services.Interfaces.Repositories;
using System.Data.Common;

namespace Market.Repositories;

internal sealed class CityRepository(DbConnection connection)
    : BaseRepository<CityDTO>(connection), ICityRepository
{
    public CityDTO? GetByName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));

        return Search(city => city.Name == name && city.IsDeleted == false)
            .FirstOrDefault();
    }

    public IEnumerable<CityDTO> GetAllActive()
        => Search(city => city.IsDeleted == false);
}