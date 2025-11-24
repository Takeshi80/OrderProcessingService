using Microsoft.EntityFrameworkCore;
using OPS.WebApi.Data.Models;

namespace OPS.WebApi.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<Inventory> Inventories => Set<Inventory>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Customer> OrderStatus => Set<Customer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<OrderItem>()
            .HasKey(o => new { o.OrderId, o.ItemId });

        modelBuilder.Entity<OrderItem>()
            .HasOne(o => o.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(o => o.OrderId);

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Item)
            .WithMany(i => i.OrderItems)
            .HasForeignKey(oi => oi.ItemId);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.Customer)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerId);
    }
}