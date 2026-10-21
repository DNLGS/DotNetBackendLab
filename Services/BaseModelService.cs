using DotNetFoundations.Filters;
using DotNetFoundations.Repository;
using Microsoft.EntityFrameworkCore;

namespace DotNetFoundations.Services
{
    public class BaseModelService<T> where T : class
    {
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
               IQueryable<T> query,BaseFilter filter)
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

            return ordered
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
        }

        private IQueryable<T> GetQuery()
        {
            return _repo.GetQuery<T>();
        }

        public async Task<List<T>> GetByFilterAsync(BaseFilter filter)
        {
            ArgumentNullException.ThrowIfNull(filter);
            var query = GetQuery();            
            query = ApplyQueryOptions(query, filter);
            return await query.ToListAsync<T>();
        }

        public virtual async Task<int> AddAsync(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            try
            {
                _repo.Add<T>(entity);
                return await _databaseService.SaveChangesAsync();

            }
            catch (Exception)
            {
                if (_databaseService.HasActiveTransaction)
                {
                    await _databaseService.RollbackTransactionAsync();
                }
                throw;
            }
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

        public virtual async Task<int> UpdateAsync(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            ValidatePrimaryKeys(entity);
            try
            {
                _repo.Update<T>(entity);
                return await _databaseService.SaveChangesAsync();

            }
            catch (Exception)
            {
                if (_databaseService.HasActiveTransaction)
                {
                    await _databaseService.RollbackTransactionAsync();
                }
                throw;
            }
        }

        public virtual async Task<int> DeleteAsync(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            ValidatePrimaryKeys(entity);
            try
            {
                _repo.Delete<T>(entity);
                return await _databaseService.SaveChangesAsync();
            }
            catch (Exception)
            {
                if (_databaseService.HasActiveTransaction)
                {
                    await _databaseService.RollbackTransactionAsync();
                }
                throw;
            }
        }
    }
}
