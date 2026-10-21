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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Clients>().HasKey(client => client.ClientsId);
            modelBuilder.Entity<Products>().HasKey(product => product.ProductsId);
            modelBuilder.Entity<Orders>().HasKey(order => order.OrdersId);
            modelBuilder.Entity<Items>().HasKey(item => item.ItemsId);

            modelBuilder.Entity<Orders>()
                .HasOne(order => order.Client)
                .WithMany(client => client.Orders)
                .HasForeignKey(order => order.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Orders>()
                .HasMany(order => order.Items)
                .WithOne()
                .HasForeignKey(item => item.OrdersId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<Items>()
                .HasOne(item => item.Product)
                .WithMany()
                .HasForeignKey(item => item.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
