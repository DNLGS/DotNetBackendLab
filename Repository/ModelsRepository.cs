using DotNetFoundations.Database;
using DotNetFoundations.DTOs.Common;
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

        public async Task<List<T>>? GetByFilter<T>(QueryRequest filter) where T : class
        {
            var query = GetQuery<T>();
            return await query.ToListAsync<T>();
        }

        public void Add<T>(T entity) where T : class
        {
            ValidateEntity(entity);
            _db.Set<T>().Add(entity);
        }

        public void Update<T>(T entity) where T : class
        {
            ValidateEntity(entity);
            _db.Set<T>().Update(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            ValidateEntity(entity);
            _db.Set<T>().Remove(entity);
        }
    }
}
