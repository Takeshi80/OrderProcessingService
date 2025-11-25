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
                new Item() { Name = "Macarons" },
                new Item() { Name = "Potato" },
                new Item() { Name = "Garland" },
                new Item() { Name = "Garland" },
                new Item() { Name = "Chocolate" },
                new Item() { Name = "Mix kit" },
                new Item() { Name = "Mug" },
                new Item() { Name = "Plant" },
                new Item() { Name = "Cake" },
            ]);
        }

        await context.SaveChangesAsync();
        
        if (!context.Customers.Any())
        {
            context.Customers.AddRange(
            [
                new Customer() { FirstName = "John", LastName = "Doe" },
                new Customer() { FirstName = "Baron", LastName = "Megatron" },
                new Customer() { FirstName = "James", LastName = "Collider" },
                new Customer() { FirstName = "Petro", LastName = "Schur" },
                new Customer() { FirstName = "Ivan", LastName = "Minigun" },
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