using Microsoft.EntityFrameworkCore;
using ZakupekApi.Db.Models;

namespace ZakupekApi.Db.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<ShoppingList> ShoppingLists { get; set; }
    public DbSet<ProductStatus> ProductStatuses { get; set; }
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ProductStatus>().HasData(
            new ProductStatus { Id = 1, Name = "AI generated" },
            new ProductStatus { Id = 2, Name = "Partially by AI" },
            new ProductStatus { Id = 3, Name = "Manual" },
            new ProductStatus { Id = 4, Name = "Deleted" }
        );
    }
}
