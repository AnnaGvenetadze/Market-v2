using System.Data.Common;
using Market.DTO;
using Market.Services.Interfaces.Repositories;

namespace Market.Repositories;

internal sealed class CountryRepository(DbConnection connection)
    : BaseRepository<CountryDTO>(connection), ICountryRepository
{
    public CountryDTO? GetByCode(string countryCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(countryCode);
        var trimmedCode = countryCode.Trim();
        return Search(c => c.CountryCode == trimmedCode)
            .FirstOrDefault();
    }
}