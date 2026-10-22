using System.ComponentModel.DataAnnotations;
using DotNetFoundations.DTOs.Orders.Requests;
using DotNetFoundations.DTOs.Orders.Responses;
using DotNetFoundations.DTOs.Common;
using DotNetFoundations.Models;
using DotNetFoundations.Repository;
using Microsoft.EntityFrameworkCore;

namespace DotNetFoundations.Services
{
    public class OrderService : BaseModelService<Orders>
    {
        public override Orders CreateObjectByDTO<Dto>(Dto dto)
        {
            ValidateDto(dto);
            var order = dto switch
            {
                CreateOrderRequest create => new Orders
                {
                    ClientId = create.ClientId,
                    Status = create.Status,
                    Descount = create.Descount,
                    Increase = create.Increase,
                    Items = ConvertItems(create.Items)
                },
                UpdateOrderRequest update => new Orders
                {
                    OrdersId = update.OrdersId,
                    ClientId = update.ClientId,
                    Status = update.Status,
                    Descount = update.Descount,
                    Increase = update.Increase,
                    Items = ConvertItems(update.Items)
                },
                _ => throw new ArgumentException("DTO type not supported.", nameof(dto))
            };

            if (order.Items.Select(item => item.ItemsId).Distinct().Count() != order.Items.Count)
                throw new ValidationException("A lista de itens não pode conter identificadores duplicados.");

            try
            {
                order.GrossValue = order.Items.Sum(item => item.Total);
                _ = order.Total;
            }
            catch (OverflowException)
            {
                throw new ValidationException("Os valores do pedido excedem o limite decimal.");
            }

            return order;
        }

        private static List<Items> ConvertItems<Dto>(List<Dto>? items)
        {
            if (items is null || items.Any(item => item is null))
                throw new ValidationException("A lista de itens deve ser informada e não pode conter itens nulos.");

            return items.Select(ItemsService.ToEntity).ToList();
        }

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
            var order = CreateObjectByDTO(request);
            await ValidateReferencesAsync(order);
            await AddAsync(order);
        }

        private async Task ValidateReferencesAsync(Orders order)
        {
            if (!await _databaseService.Context.Clients.AnyAsync(client => client.ClientsId == order.ClientId))
                throw new ArgumentException("Cliente informado não existe.");

            var productIds = order.Items.Select(item => item.ProductId).Distinct().ToList();
            var existingProducts = await _databaseService.Context.Products
                .CountAsync(product => productIds.Contains(product.ProductsId));

            if (existingProducts != productIds.Count)
                throw new ArgumentException("Um ou mais produtos informados não existem.");
        }

        public async Task<int> UpdateAsync(UpdateOrderRequest request)
        {
            var changes = CreateObjectByDTO(request);
            var order = await _databaseService.Context.Orders
                .Include(entity => entity.Items)
                .SingleOrDefaultAsync(entity => entity.OrdersId == changes.OrdersId)
                ?? throw new KeyNotFoundException("Order not found");

            var existingItems = order.Items.ToDictionary(item => item.ItemsId);
            if (changes.Items.Any(item => !existingItems.ContainsKey(item.ItemsId)))
                throw new ValidationException("Um ou mais itens informados não pertencem ao pedido.");

            await ValidateReferencesAsync(changes);

            order.ClientId = changes.ClientId;
            order.Status = changes.Status;
            order.Descount = changes.Descount;
            order.Increase = changes.Increase;
            order.GrossValue = changes.GrossValue;

            var requestedIds = changes.Items.Select(item => item.ItemsId).ToHashSet();
            var removedItems = order.Items.Where(item => !requestedIds.Contains(item.ItemsId)).ToList();
            // O relacionamento usa ClientSetNull; remover explicitamente evita deixar itens órfãos.
            _databaseService.Context.Items.RemoveRange(removedItems);
            foreach (var item in removedItems)
                order.Items.Remove(item);

            foreach (var item in changes.Items)
            {
                var existing = existingItems[item.ItemsId];
                existing.ProductId = item.ProductId;
                existing.Quantity = item.Quantity;
                existing.UnitPrice = item.UnitPrice;
            }

            return await base.UpdateAsync(order);
        }

        public async Task<List<OrderResponse>> QueryAsync(QueryOrderRequest request)
        {
            var entities = await GetByFilterAsync(request);
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

        public override IQueryable<Orders> ApplyQueryOptions(IQueryable<Orders> query, QueryRequest filter)
        {
            query = query.Include(o => o.Client);
            query = query.Include(o => o.Items)
                         .ThenInclude(i => i.Product);
            if (filter is QueryOrderRequest orderFilter)
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
