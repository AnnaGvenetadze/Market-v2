using Market.DTO;

namespace Market.Services.Interfaces.Repositories;

public interface ICorporateClientDetailsRepository : IBaseRepository<CorporateClientDetailsDTO>
{
    CorporateClientDetailsDTO? GetByTaxNumber(string taxNumber);
    IEnumerable<CorporateClientDetailsDTO> GetDeletedCorporateClientDetails();
}