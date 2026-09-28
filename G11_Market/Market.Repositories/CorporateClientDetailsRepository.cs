using System.Data.Common;
using Market.DTO;
using Market.Services.Interfaces.Repositories;

namespace Market.Repositories;

internal sealed class CorporateClientDetailsRepository(DbConnection connection)
    : BaseRepository<CorporateClientDetailsDTO>(connection), ICorporateClientDetailsRepository
{
    public CorporateClientDetailsDTO? GetByTaxNumber(string taxNumber)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(taxNumber);
        var trimmedTaxNumber = taxNumber.Trim();
        return Search(client => client.TaxNumber == trimmedTaxNumber)
            .FirstOrDefault();
    }

    public IEnumerable<CorporateClientDetailsDTO> GetDeletedCorporateClientDetails()
    {
        return Search(client => client.IsDeleted == true);
    }
}