using System.ComponentModel.DataAnnotations;
using DotNetFoundations.DTOs.Items.Requests;
using DotNetFoundations.DTOs.Items.Responses;
using DotNetFoundations.Models;
using DotNetFoundations.Repository;

namespace DotNetFoundations.Services
{
    public class ItemsService : BaseModelService<Items>
    {
        public override Items CreateObjectByDTO<Dto>(Dto dto) => ToEntity(dto);

        public ItemsService(
            ModelsRepository repository,
            DatabaseService databaseService)
            : base(repository, databaseService)
        { }

        internal static Items ToEntity<Dto>(Dto request)
        {
            ValidateDto(request);
            var item = request switch
            {
                CreateItemRequest create => new Items
                {
                    ProductId = create.ProductId,
                    Quantity = create.Quantity,
                    UnitPrice = create.UnitPrice
                },
                UpdateItemRequest update => new Items
                {
                    ItemsId = update.ItemsId,
                    ProductId = update.ProductId,
                    Quantity = update.Quantity,
                    UnitPrice = update.UnitPrice
                },
                _ => throw new ArgumentException("DTO type not supported.", nameof(request))
            };

            try
            {
                _ = item.Total;
            }
            catch (OverflowException)
            {
                throw new ValidationException("Os valores do item excedem o limite decimal.");
            }

            return item;
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
