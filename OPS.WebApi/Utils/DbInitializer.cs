using OPS.Data;
using OPS.Data.Models;

namespace OPS.WebApi.Utils;

public static class DbInitializer
{
    public static async Task Seed(AppDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        if (!context.Items.Any())
        {
            context.Items.AddRange(
            [
                new Item { Name = "Macarons", UnitPrice = 105.32m },
                new Item { Name = "Potato", UnitPrice = 100.21m },
                new Item { Name = "Garland", UnitPrice = 107.82m },
                new Item { Name = "Garland", UnitPrice = 92.13m },
                new Item { Name = "Chocolate", UnitPrice = 33.74m },
                new Item { Name = "Mix kit", UnitPrice = 14.52m },
                new Item { Name = "Mug", UnitPrice = 123.99m },
                new Item { Name = "Plant", UnitPrice = 22.23m },
                new Item { Name = "Cake", UnitPrice = 11.37m },
            ]);
        }

        await context.SaveChangesAsync();

        if (!context.Customers.Any())
        {
            context.Customers.AddRange(
            [
                new Customer { FirstName = "John", LastName = "Doe" },
                new Customer { FirstName = "Baron", LastName = "Megatron" },
                new Customer { FirstName = "James", LastName = "Collider" },
                new Customer { FirstName = "Petro", LastName = "Schur" },
                new Customer { FirstName = "Ivan", LastName = "Minigun" },
            ]);
        }

        if (!context.Inventories.Any())
        {
            var items = context.Items.ToList();
            foreach (var item in items)
            {
                context.Inventories.Add(new Inventory { ItemId = item.Id, AvailableAmount = Random.Shared.Next(100) });
            }
        }

        await context.SaveChangesAsync();
    }
}