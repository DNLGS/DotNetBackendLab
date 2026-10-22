
using DotNetFoundations.Database;

namespace DotNetFoundations.Services
{
    public class DatabaseService
    {
        private readonly DatabaseContext _db;

        public DatabaseService(DatabaseContext dbcontext)
        {
            _db = dbcontext ??
                throw new ArgumentNullException(nameof(dbcontext));
        }
        public DatabaseContext Context => _db;

        public Task<bool> CheckConnection(CancellationToken ct = default)
        {
            return _db.Database.CanConnectAsync(ct);
        }

        public Task<int> SaveChangesAsync()
        {
            return _db.SaveChangesAsync();
        }
    }
}
