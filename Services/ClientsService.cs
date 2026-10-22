using System.ComponentModel.DataAnnotations;
using DotNetFoundations.DTOs.Clients.Requests;
using DotNetFoundations.DTOs.Clients.Responses;
using DotNetFoundations.DTOs.Common;
using DotNetFoundations.Models;
using DotNetFoundations.Repository;

namespace DotNetFoundations.Services
{
    public class ClientsService : BaseModelService<Clients>
    {
        public override Clients CreateObjectByDTO<Dto>(Dto dto)
        {
            ValidateDto(dto);
            if (dto is UpdateClientRequest { Name: not null } update && string.IsNullOrWhiteSpace(update.Name))
                throw new ValidationException("O nome do cliente não pode estar em branco.");

            return dto switch
            {
                CreateClientRequest create => new Clients
                {
                    Name = create.Name,
                    Email = create.Email,
                    Phone = create.Phone
                },
                UpdateClientRequest request => new Clients
                {
                    ClientsId = request.ClientsId,
                    Name = request.Name ?? string.Empty,
                    Email = request.Email,
                    Phone = request.Phone
                },
                _ => throw new ArgumentException("DTO type not supported.", nameof(dto))
            };
        }

        public ClientsService(
            ModelsRepository repository,
            DatabaseService databaseService)
            : base(repository, databaseService)
        {
        }

        internal static ClientResponse ToResponse(Clients entity)
        {
            return new ClientResponse
            {
                ClientsId = entity.ClientsId,
                Version = entity.Version,
                Name = entity.Name,
                Email = entity.Email,
                Phone = entity.Phone,
                CreatedAt = entity.CreatedAt
            };
        }

        public override IQueryable<Clients> ApplyQueryOptions(IQueryable<Clients> query, QueryRequest filter)
        {
            if (filter is QueryClientRequest clientFilter)
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
