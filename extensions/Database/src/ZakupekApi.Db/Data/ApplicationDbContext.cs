using Microsoft.EntityFrameworkCore;
using ZakupekApi.Db.Models;

namespace ZakupekApi.Db.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<ShoppingList> ShoppingLists { get; set; }
    public DbSet<Status> ProductStatuses { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<UserDietaryPreference> UserDietaryPreferences { get; set; }
    public DbSet<UserAge> UserAges { get; set; }
    public DbSet<Store> Stores { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed ProductStatuses
        modelBuilder.Entity<Status>().HasData(
            new Status { Id = 1, Name = "AI generated" },
            new Status { Id = 2, Name = "Partially by AI" },
            new Status { Id = 3, Name = "Manual" },
            new Status { Id = 4, Name = "Deleted" }
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

        modelBuilder.Entity<Store>()
            .HasOne(s => s.User)
            .WithMany(u => u.Stores)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ShoppingList>()
            .HasOne(sl => sl.Store)
            .WithMany(s => s.ShoppingLists)
            .HasForeignKey(sl => sl.StoreId)
            .OnDelete(DeleteBehavior.SetNull);

        // Configure indexes according to the plan
        modelBuilder.Entity<ShoppingList>()
            .HasIndex(sl => sl.UserId);

        modelBuilder.Entity<Product>()
            .HasIndex(p => p.ShoppingListId);

        modelBuilder.Entity<UserDietaryPreference>()
            .HasIndex(udp => udp.UserId);

        modelBuilder.Entity<UserAge>()
            .HasIndex(ua => ua.UserId);

        modelBuilder.Entity<Store>()
            .HasIndex(s => s.UserId);
    }
}
