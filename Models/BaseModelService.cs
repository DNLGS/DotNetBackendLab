using DotNetFoundations.Repository;
using DotNetFoundations.Services;

namespace DotNetFoundations.Models
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

        public virtual async Task<int> UpdateAsync(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
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
