using DotNetFoundations.DTOs.Clients.Requests;
using DotNetFoundations.DTOs.Clients.Responses;
using DotNetFoundations.Filters;
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

        public async Task CreateAsync(CreateClientRequest request)
        {
            await AddAsync(new Clients
            {
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone
            });
        }

        public async Task<List<ClientResponse>> QueryAsync(QueryClientRequest request)
        {
            var filter = new ClientFilter
            {
                Page = request.Page,
                PageSize = request.PageSize,
                OrderBy = request.OrderBy,
                Descending = request.Descending,
                Name = request.Name,
                NameContains = request.NameContains,
                Email = request.Email,
                EmailContains = request.EmailContains,
                Phone = request.Phone
            };
            var entities = await GetByFilterAsync(filter);
            return entities.Select(ToResponse).ToList();
        }

        internal static ClientResponse ToResponse(Clients entity)
        {
            return new ClientResponse
            {
                ClientsId = entity.ClientsId,
                Name = entity.Name,
                Email = entity.Email,
                Phone = entity.Phone,
                CreatedAt = entity.CreatedAt
            };
        }

        public override IQueryable<Clients> ApplyQueryOptions(IQueryable<Clients> query, BaseFilter filter)
        {
            if (filter is ClientFilter clientFilter)
            {
                if (!string.IsNullOrEmpty(clientFilter.Name))
                {
                    query = query.Where(p => p.Name.Equals(clientFilter.Name));
                }
                if (!string.IsNullOrEmpty(clientFilter.NameContains))
                {
                    query = query.Where(p => p.Name.Contains(clientFilter.NameContains));
                }
                if (!string.IsNullOrEmpty(clientFilter.Email))
                {
                    query = query.Where(p => p.Email == clientFilter.Email);
                }
                if (!string.IsNullOrWhiteSpace(clientFilter.EmailContains))
                {
                    query = query.Where(p =>
                        p.Email != null &&
                        p.Email.Contains(clientFilter.EmailContains));
                }
                if (!string.IsNullOrEmpty(clientFilter.Phone))
                {
                    query = query.Where(p => p.Phone == clientFilter.Phone);
                }
            }

            return base.ApplyQueryOptions(query, filter);
        }
    }
}
