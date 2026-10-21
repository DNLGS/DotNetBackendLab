
using DotNetFoundations.Database;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data.Common;
using System.Linq.Expressions;

namespace DotNetFoundations.Services
{
    public class DatabaseService : IAsyncDisposable
    {
        private readonly DatabaseContext _db;
        private IDbContextTransaction? _transaction;
        private bool _disposed;

        public DatabaseService(DatabaseContext dbcontext)
        {
            _db = dbcontext ??
                throw new ArgumentNullException(nameof(dbcontext));
        }
        public DatabaseContext Context => _db;

        public async Task<bool> CheckConnection(CancellationToken ct = default)
        {
            return await _db.Database.CanConnectAsync(ct);
        }

        public bool HasActiveTransaction =>
            _transaction != null;

        public async Task<int> SaveChangesAsync()
        {
            ValidateNotDisposed();
            return await _db.SaveChangesAsync();            
        }

        public async Task RollbackTransactionAsync()
        {
            ValidateNotDisposed();

            if (_transaction == null)
                throw new InvalidOperationException(
                    "Nenhuma transação ativa.");

            try
            {
                await _transaction.RollbackAsync();
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        private void ValidateNotDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(DatabaseService));
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed)
                return;

            _disposed = true;

            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }
}
