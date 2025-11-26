using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OPS.Data.Models;

namespace OPS.Data.Repositories;

public interface IInventoryRepository
{
    Task EnsureInventoryAsync(List<OrderItemDto> orderItems);
    Task<IReadOnlyList<Inventory>> List();
}

public class InventoryRepository(
    AppDbContext dbContext,
    ILogger<InventoryRepository> logger)
    : EfRepository<Inventory>(dbContext), IInventoryRepository
{
    public async Task<IReadOnlyList<Inventory>> List()
    {
        return await ListAsync();
    }

    public async Task EnsureInventoryAsync(List<OrderItemDto> orderItems)
    {
        await using var tx = await DbContext.Database.BeginTransactionAsync();

        foreach (var item in orderItems)
        {
            var affected = await DbContext.Database.ExecuteSqlInterpolatedAsync($@"
        UPDATE ""Inventories""
        SET ""AvailableAmount"" = ""AvailableAmount"" - {item.Quantity}
        WHERE ""ItemId"" = {item.ItemId}
          AND ""AvailableAmount"" >= {item.Quantity};
    ");

            if (affected == 0)
            {
                throw new NotEnoughInventoryException($"Not enough items in inventory for item {item.ItemId}");
            }
        }

        await tx.CommitAsync();
    }
}

public class NotEnoughInventoryException(string s) : Exception(s);