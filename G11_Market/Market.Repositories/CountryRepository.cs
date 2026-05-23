using System.Data.Common;
using Market.DTO;
using Market.DTO.DbEntities.InsertEntities;
using Market.DTO.DbEntities.UpdateEntities;
using Market.Repositories.Interfaces;

namespace Market.Repositories;

public sealed class CountryRepository(DbConnection connection, bool keepConnectionOpen) : BaseRepository<CountryDTO, CountryInsert, CountryUpdate>(connection, keepConnectionOpen), ICountryRepository
{
}