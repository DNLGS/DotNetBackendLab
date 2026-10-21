using DotNetFoundations.DTOs.Items.Requests;
using DotNetFoundations.DTOs.Items.Responses;
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

        internal static Items ToEntity(CreateItemRequest request)
        {
            return new Items
            {
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                UnitPrice = request.UnitPrice
            };
        }

        internal static ItemResponse ToResponse(Items entity)
        {
            return new ItemResponse
            {
                ItemsId = entity.ItemsId,
                OrdersId = entity.OrdersId,
                ProductId = entity.ProductId,
                Product = entity.Product is null ? null : ProductsService.ToResponse(entity.Product),
                Quantity = entity.Quantity,
                UnitPrice = entity.UnitPrice,
                Total = entity.Total,
                CreatedAt = entity.CreatedAt
            };
        }
    }
}
