using System.Data.Common;
using Market.DTO;
using Market.Services.Interfaces.Repositories;

namespace Market.Repositories;

public sealed class ClientRepository(DbConnection connection) 
    : BaseRepository<ClientDTO>(connection), IClientRepository
{
}