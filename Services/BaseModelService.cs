using DotNetFoundations.DTOs.Common;
using DotNetFoundations.Interfaces;
using System.ComponentModel.DataAnnotations;
using DotNetFoundations.Repository;
using Microsoft.EntityFrameworkCore;

namespace DotNetFoundations.Services
{
    public abstract class BaseModelService<T>
            where T : class
    {
        public abstract T CreateObjectByDTO<Dto>(Dto dto);

        protected static void ValidateDto(object? dto)
        {
            ArgumentNullException.ThrowIfNull(dto);
            Validator.ValidateObject(dto, new ValidationContext(dto), validateAllProperties: true);
        }
        private readonly ModelsRepository _repo;
        private readonly DatabaseService _databaseService;
        public BaseModelService(
            ModelsRepository repository,
            DatabaseService databaseService)
        {
            _repo = repository ?? throw new ArgumentNullException(nameof(repository));
            _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        }

        public virtual IQueryable<T> ApplyQueryOptions(
               IQueryable<T> query,QueryRequest filter)
        {
            var entityType = _databaseService.Context.Model.FindEntityType(typeof(T))
                ?? throw new InvalidOperationException(
                    "Entidade não mapeada.");

            var primaryKey = entityType.FindPrimaryKey()
                ?? throw new InvalidOperationException(
                    "Entidade sem chave primária.");

            if (primaryKey.Properties.Count != 1)
                throw new NotSupportedException(
                    "Este método exige uma chave primária simples.");

            var keyName = primaryKey.Properties[0].Name;

            var orderBy = string.IsNullOrWhiteSpace(filter.OrderBy)
                ? keyName
                : filter.OrderBy;

            var property = entityType.FindProperty(orderBy)
                ?? throw new ArgumentException(
                    $"Propriedade de ordenação inválida: {orderBy}");

            if (property.IsConcurrencyToken)
                throw new ArgumentException("O token de versão não pode ser usado na ordenação.");

            var ordered = filter.Descending
                ? query.OrderByDescending(
                    e => EF.Property<object>(e, property.Name))
                : query.OrderBy(
                    e => EF.Property<object>(e, property.Name));

            // Desempate pela chave primária
            if (property.Name != keyName)
            {
                ordered = ordered.ThenBy(
                    e => EF.Property<object>(e, keyName));
            }

            int page = Math.Max(1, filter.Page);
            int pageSize = Math.Clamp(filter.PageSize, 1, 100);
            long offset = (long)(page - 1) * pageSize;
            if (offset > int.MaxValue)
                throw new ValidationException("A página informada excede o limite de paginação.");

            return ordered
                .Skip((int)offset)
                .Take(pageSize);
        }

        private IQueryable<T> GetQuery()
        {
            return _repo.GetQuery<T>();
        }

        public async Task<List<T>> GetByFilterAsync(QueryRequest filter)
        {
            ArgumentNullException.ThrowIfNull(filter);
            var query = GetQuery();            
            query = ApplyQueryOptions(query, filter);
            return await query.ToListAsync<T>();
        }

        public Task<int> AddAsync<TDto>(TDto dto)
        {
            return AddAsync(CreateObjectByDTO(dto));
        }

        protected virtual Task PrepareAddAsync(T entity)
        {
            return Task.CompletedTask;
        }

        public virtual async Task<int> AddAsync(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            await PrepareAddAsync(entity);
            _repo.Add(entity);
            return await _databaseService.SaveChangesAsync();
        }

        public void ValidatePrimaryKeys(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            var entityType = _databaseService.Context.Model
                .FindEntityType(typeof(T))
                ?? throw new InvalidOperationException("Entidade não mapeada.");

            var primaryKey = entityType.FindPrimaryKey()
                ?? throw new InvalidOperationException("Entidade sem chave primária.");

            foreach (var key in primaryKey.Properties)
            {
                var propertyInfo = key.PropertyInfo
                    ?? throw new InvalidOperationException(
                        $"Propriedade {key.Name} não encontrada.");

                var value = propertyInfo.GetValue(entity);

                bool isInvalid = value switch
                {
                    null => true,
                    Guid guid => guid == Guid.Empty,
                    string str => string.IsNullOrWhiteSpace(str),
                    _ => false
                };

                if (isInvalid)
                {
                    throw new ArgumentException(
                        $"A chave primária '{key.Name}' não possui um valor válido.");
                }
            }
        }

        public async Task<int> UpdateAsync<TDto>(TDto dto) where TDto : IVersionedRequest
        {
            var changes = CreateObjectByDTO(dto);
            ValidatePrimaryKeys(changes);

            var entry = _databaseService.Context.Entry(changes);
            var primaryKey = entry.Metadata.FindPrimaryKey()
                ?? throw new InvalidOperationException("Entidade sem chave primária.");
            var keyValues = primaryKey.Properties
                .Select(key => entry.Property(key.Name).CurrentValue!)
                .ToArray();

            var entity = await GetByIdAsync(keyValues)
                ?? throw new KeyNotFoundException($"{typeof(T).Name} não encontrado.");

            ValidateVersion(entity, dto.Version);
            await PrepareUpdateAsync(entity, changes);
            _repo.ApplyChanges(entity, dto);
            return await _databaseService.SaveChangesAsync();
        }

        protected virtual Task PrepareUpdateAsync(T entity, T changes)
        {
            return Task.CompletedTask;
        }

        private void ValidateVersion(T entity, uint version)
        {
            if (version == 0)
                throw new ValidationException("Informe a versão recebida na consulta do registro.");

            var property = _databaseService.Context.Entry(entity)
                .Property<uint>(nameof(IVersionedRequest.Version));
            if (property.CurrentValue != version)
                throw new DbUpdateConcurrencyException("O registro foi alterado. Consulte novamente antes de salvar ou excluir.");

            property.OriginalValue = version;
        }

        public virtual async Task<int> DeleteAsync(T entity, uint version)
        {
            ArgumentNullException.ThrowIfNull(entity);
            ValidatePrimaryKeys(entity);
            ValidateVersion(entity, version);
            _repo.Delete<T>(entity);
            return await _databaseService.SaveChangesAsync();
        }

        public virtual async Task<T?> GetByIdAsync(params object[] id)
        {
            ArgumentNullException.ThrowIfNull(id);
            return await _databaseService.Context.Set<T>().FindAsync(id);
        }
    }
}
