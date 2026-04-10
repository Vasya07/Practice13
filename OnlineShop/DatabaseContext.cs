using System.Data.Entity;
using OnlineShop.Models;

namespace OnlineShop
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext() : base("name=OnlineShopDB")
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<DatabaseContext>());
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}