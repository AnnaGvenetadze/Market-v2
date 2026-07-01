using Market.DTO;

namespace Market.Services.Interfaces.Repositories;

public interface ICityRepository : IBaseRepository<CityDTO>
{
    public CityDTO? GetByName(string name);
    public IEnumerable<CityDTO> GetAllActive();
}