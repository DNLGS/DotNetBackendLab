using DotNetFoundations.Models;
using DotNetFoundations.Repository;

namespace DotNetFoundations.Services
{
    public class ItemsService : BaseModelService<Items>
    {
        public ItemsService(
            ModelsRepository repository,
            DatabaseService databaseService)
            : base(repository, databaseService)
        { }
    }
}
