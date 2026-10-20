using DotNetFoundations.Models;
using DotNetFoundations.Repository;

namespace DotNetFoundations.Services
{
    public class OrderService : BaseModelService<Orders>
    {
        public OrderService(
            ModelsRepository repository,
            DatabaseService databaseService)
            : base(repository, databaseService)
        {
        }
    }
}
