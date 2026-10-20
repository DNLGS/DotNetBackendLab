using DotNetFoundations.Models;
using Microsoft.EntityFrameworkCore;

namespace DotNetFoundations.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Clients> Clients { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<Items> Items { get; set; }        

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }
    }
}
