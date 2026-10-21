using DotNetFoundations.DTOs.Products.Requests;
using DotNetFoundations.DTOs.Products.Responses;
using DotNetFoundations.Filters;
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

        public async Task CreateAsync(CreateProductRequest request)
        {
            await AddAsync(new Products
            {
                Name = request.Name,
                SuggestedPrice = request.SuggestedPrice
            });
        }

        public async Task<List<ProductResponse>> QueryAsync(QueryProductRequest request)
        {
            var filter = new ProductFilter
            {
                Page = request.Page,
                PageSize = request.PageSize,
                OrderBy = request.OrderBy,
                Descending = request.Descending,
                Name = request.Name,
                NameContains = request.NameContains,
                MinSuggestedPrice = request.MinSuggestedPrice
            };
            var entities = await GetByFilterAsync(filter);
            return entities.Select(ToResponse).ToList();
        }

        internal static ProductResponse ToResponse(Products entity)
        {
            return new ProductResponse
            {
                ProductsId = entity.ProductsId,
                Name = entity.Name,
                SuggestedPrice = entity.SuggestedPrice,
                CreatedAt = entity.CreatedAt
            };
        }

        public override IQueryable<Products> ApplyQueryOptions(IQueryable<Products> query, BaseFilter filter)
        {
            if (filter is ProductFilter productsFilter)
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
