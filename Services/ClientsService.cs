using DotNetFoundations.Models;
using DotNetFoundations.Repository;

namespace DotNetFoundations.Services
{
    public class ClientsService : BaseModelService<Clients>
    {
        public ClientsService(
            ModelsRepository repository,
            DatabaseService databaseService)
            : base(repository, databaseService)
        {
        }
    }
}
