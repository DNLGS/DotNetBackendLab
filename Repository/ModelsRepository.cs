using DotNetFoundations.Database;
using Microsoft.EntityFrameworkCore;

namespace DotNetFoundations.Repository
{
    public class ModelsRepository
    {
        private readonly DatabaseContext _db;
        public ModelsRepository(DatabaseContext db) 
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        private void ValidateEntity<T>(T entity) where T : class
        {
            ArgumentNullException.ThrowIfNull(entity);
            if (_db.Model.FindEntityType(typeof(T)) == null)
                throw new InvalidOperationException(
                    $"A entidade {typeof(T).Name} não está mapeada.");
        }

        public IQueryable<T> GetQuery<T>() where T : class
        {
            var query = _db.Set<T>().AsQueryable();            
            return query;
        }

        public void Add<T>(T entity) where T : class
        {
            ValidateEntity(entity);
            _db.Set<T>().Add(entity);
        }

        public void ApplyChanges<T, TDto>(T entity, TDto dto) where T : class
        {
            ValidateEntity(entity);
            ArgumentNullException.ThrowIfNull(dto);

            var entry = _db.Entry(entity);
            var values = entry.CurrentValues.Clone();
            values.SetValues(dto);

            foreach (var property in entry.Properties)
            {
                // Campos nulos preservam o valor atual, como na atualização parcial de clientes.
                if (!property.Metadata.IsPrimaryKey() && !property.Metadata.IsConcurrencyToken &&
                    values[property.Metadata.Name] is { } value)
                    property.CurrentValue = value;
            }
        }

        public void Delete<T>(T entity) where T : class
        {
            ValidateEntity(entity);
            _db.Set<T>().Remove(entity);
        }
    }
}
