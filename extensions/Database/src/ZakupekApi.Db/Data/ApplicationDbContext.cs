using Microsoft.EntityFrameworkCore;
using ZakupekApi.Db.Models;

namespace ZakupekApi.Db.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<ShoppingList> ShoppingLists { get; set; }
    public DbSet<ProductStatus> ProductStatuses { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<UserDietaryPreference> UserDietaryPreferences { get; set; }
    public DbSet<UserAge> UserAges { get; set; }
    public DbSet<Section> Sections { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed ProductStatuses
        modelBuilder.Entity<ProductStatus>().HasData(
            new ProductStatus { Id = 1, Name = "AI generated" },
            new ProductStatus { Id = 2, Name = "Partially by AI" },
            new ProductStatus { Id = 3, Name = "Manual" },
            new ProductStatus { Id = 4, Name = "Deleted" }
        );

        // Configure relationships
        modelBuilder.Entity<ShoppingList>()
            .HasOne(sl => sl.User)
            .WithMany(u => u.ShoppingLists)
            .HasForeignKey(sl => sl.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.ShoppingList)
            .WithMany(sl => sl.Products)
            .HasForeignKey(p => p.ShoppingListId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Status)
            .WithMany(ps => ps.Products)
            .HasForeignKey(p => p.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Section)
            .WithMany(s => s.Products)
            .HasForeignKey(p => p.SectionId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<UserDietaryPreference>()
            .HasOne(udp => udp.User)
            .WithMany(u => u.DietaryPreferences)
            .HasForeignKey(udp => udp.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserAge>()
            .HasOne(ua => ua.User)
            .WithMany(u => u.Ages)
            .HasForeignKey(ua => ua.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Section>()
            .HasOne(s => s.User)
            .WithMany(u => u.Sections)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure indexes according to the plan
        modelBuilder.Entity<ShoppingList>()
            .HasIndex(sl => sl.UserId);

        modelBuilder.Entity<Product>()
            .HasIndex(p => p.ShoppingListId);

        modelBuilder.Entity<Product>()
            .HasIndex(p => p.SectionId);

        modelBuilder.Entity<Product>()
            .HasIndex(p => new { p.ShoppingListId, p.SectionId, p.OrderInSection });

        modelBuilder.Entity<UserDietaryPreference>()
            .HasIndex(udp => udp.UserId);

        modelBuilder.Entity<UserAge>()
            .HasIndex(ua => ua.UserId);

        modelBuilder.Entity<Section>()
            .HasIndex(s => s.UserId);
    }
}
