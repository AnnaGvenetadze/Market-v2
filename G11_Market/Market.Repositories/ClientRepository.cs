using System.Data.Common;
using Market.DTO;
using Market.Repositories.Interfaces;

namespace Market.Repositories;

public sealed class ClientRepository(DbConnection connection) : BaseRepository<ClientDTO>(connection), IClientRepository
{
}