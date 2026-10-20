using DotNetFoundations.Models;
using DotNetFoundations.Repository;

namespace DotNetFoundations.Services
{
    public class ProductsService : BaseModelService<Products>
    {
        public ProductsService(
            ModelsRepository repository,
            DatabaseService databaseService)
            : base(repository, databaseService)
        {
        }
    }
}
