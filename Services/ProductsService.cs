using DotNetFoundations.DTOs.Products.Requests;
using DotNetFoundations.DTOs.Products.Responses;
using DotNetFoundations.DTOs.Common;
using DotNetFoundations.Models;
using DotNetFoundations.Repository;

namespace DotNetFoundations.Services
{
    public class ProductsService : BaseModelService<Products>
    {
        public override Products CreateObjectByDTO<Dto>(Dto dto)
        {
            ValidateDto(dto);
            return dto switch
            {
                CreateProductRequest create => new Products
                {
                    Name = create.Name,
                    SuggestedPrice = create.SuggestedPrice
                },
                UpdateProductRequest update => new Products
                {
                    ProductsId = update.ProductsId,
                    Name = update.Name,
                    SuggestedPrice = update.SuggestedPrice
                },
                _ => throw new ArgumentException("DTO type not supported.", nameof(dto))
            };
        }

        public ProductsService(
            ModelsRepository repository,
            DatabaseService databaseService)
            : base(repository, databaseService)
        {
        }

        internal static ProductResponse ToResponse(Products entity)
        {
            return new ProductResponse
            {
                ProductsId = entity.ProductsId,
                Version = entity.Version,
                Name = entity.Name,
                SuggestedPrice = entity.SuggestedPrice,
                CreatedAt = entity.CreatedAt
            };
        }

        public override IQueryable<Products> ApplyQueryOptions(IQueryable<Products> query, QueryRequest filter)
        {
            if (filter is QueryProductRequest productsFilter)
            {
                if (!string.IsNullOrEmpty(productsFilter.Name))
                {
                    query = query.Where(p => p.Name.Equals(productsFilter.Name));
                }
                if (!string.IsNullOrEmpty(productsFilter.NameContains))
                {
                    query = query.Where(p => p.Name.Contains(productsFilter.NameContains));
                }
                if (productsFilter.MinSuggestedPrice.HasValue)
                {
                    query = query.Where(p => p.SuggestedPrice >= productsFilter.MinSuggestedPrice.Value);
                }
            }
            return base.ApplyQueryOptions(query, filter);
        }

    }
}
