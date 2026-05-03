using System.Data.Common;
using Market.DTO.DbEntities.InsertEntities;
using Market.DTO.DbEntities.UpdateEntities;
using Market.DTO.DTOs;
using Market.Repositories.Interfaces;

namespace Market.Repositories.Implementations;

public sealed class CountryRepository(DbConnection connection) : BaseRepository<CountryDTO, CountryInsert, CountryUpdate>(connection), ICountryRepository
{
}