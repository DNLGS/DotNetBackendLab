using System.ComponentModel.DataAnnotations;
using DotNetFoundations.DTOs.Orders.Requests;
using DotNetFoundations.DTOs.Orders.Responses;
using DotNetFoundations.Filters;
using DotNetFoundations.Models;
using DotNetFoundations.Repository;
using Microsoft.EntityFrameworkCore;

namespace DotNetFoundations.Services
{
    public class OrderService : BaseModelService<Orders>
    {
        private readonly DatabaseService _databaseService;
        public OrderService(
            ModelsRepository repository,
            DatabaseService databaseService)
            : base(repository, databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task CreateAsync(CreateOrderRequest request)
        {
            if (request.Items is null || request.Items.Any(item => item is null))
                throw new ValidationException("A lista de itens deve ser informada e não pode conter itens nulos.");

            if (!await _databaseService.Context.Clients.AnyAsync(client => client.ClientsId == request.ClientId))
                throw new ArgumentException("Cliente informado não existe.");

            var productIds = request.Items.Select(item => item.ProductId).Distinct().ToList();
            var existingProducts = await _databaseService.Context.Products
                .CountAsync(product => productIds.Contains(product.ProductsId));
            if (existingProducts != productIds.Count)
                throw new ArgumentException("Um ou mais produtos informados não existem.");

            var items = request.Items.Select(ItemsService.ToEntity).ToList();
            decimal grossValue;
            try
            {
                grossValue = items.Sum(item => item.Total);
                _ = checked(grossValue - request.Descount + request.Increase);
            }
            catch (OverflowException)
            {
                throw new ValidationException("Os valores do pedido excedem o limite decimal.");
            }

            await AddAsync(new Orders
            {
                ClientId = request.ClientId,
                Status = request.Status,
                Descount = request.Descount,
                Increase = request.Increase,
                GrossValue = grossValue,
                Items = items
            });
        }

        public async Task<List<OrderResponse>> QueryAsync(QueryOrderRequest request)
        {
            var filter = new OrderFilter
            {
                Page = request.Page,
                PageSize = request.PageSize,
                OrderBy = request.OrderBy,
                Descending = request.Descending,
                ClientId = request.ClientId,
                Status = request.Status,
                Descount = request.Descount,
                Increase = request.Increase,
                GrossValue = request.GrossValue,
                CreatedAt = request.CreatedAt
            };
            var entities = await GetByFilterAsync(filter);
            return entities.Select(ToResponse).ToList();
        }

        internal static OrderResponse ToResponse(Orders entity)
        {
            return new OrderResponse
            {
                OrdersId = entity.OrdersId,
                ClientId = entity.ClientId,
                Status = entity.Status,
                Descount = entity.Descount,
                Increase = entity.Increase,
                CreatedAt = entity.CreatedAt,
                GrossValue = entity.GrossValue,
                Total = entity.Total,
                Client = entity.Client is null ? null : ClientsService.ToResponse(entity.Client),
                Items = entity.Items.Select(ItemsService.ToResponse).ToList()
            };
        }

        public override IQueryable<Orders> ApplyQueryOptions(IQueryable<Orders> query, BaseFilter filter)
        {
            query = query.Include(o => o.Client);
            query = query.Include(o => o.Items)
                         .ThenInclude(i => i.Product);
            if (filter is OrderFilter orderFilter)
            {
                if (orderFilter.ClientId.HasValue)
                {
                    query = query.Where(p =>
                        p.ClientId == orderFilter.ClientId.Value);
                }

                if (orderFilter.Status.HasValue)
                {
                    query = query.Where(p =>
                        p.Status == orderFilter.Status.Value);
                }

                if (orderFilter.Descount.HasValue)
                {
                    query = query.Where(p =>
                        p.Descount == orderFilter.Descount.Value);
                }

                if (orderFilter.Increase.HasValue)
                {
                    query = query.Where(p =>
                        p.Increase == orderFilter.Increase.Value);
                }

                if (orderFilter.GrossValue.HasValue)
                {
                    query = query.Where(p =>
                        p.GrossValue == orderFilter.GrossValue.Value);
                }

                if (orderFilter.CreatedAt.HasValue)
                {
                    var date = orderFilter.CreatedAt.Value.Date;
                    var nextDate = date.AddDays(1);

                    query = query.Where(p =>
                        p.CreatedAt >= date &&
                        p.CreatedAt < nextDate);
                }
            }
            return base.ApplyQueryOptions(query, filter);
        }
    }
}
