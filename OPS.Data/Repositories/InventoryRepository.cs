using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OPS.Data.Models;
using OPS.Shared;

namespace OPS.Data.Repositories;

public interface IInventoryRepository
{
    Task<bool> DecrementInventoryAsync(int itemId, int amount);
    Task<bool> IncrementInventoryAsync(int itemId, int amount);
    Task<bool> CheckInventoryAsync(int itemId, int amount);
    Task EnsureInventoryAsync(List<ProcessOrderItemDto> orderItems);
}

public class InventoryRepository(
    AppDbContext dbContext,
    ILogger<InventoryRepository> logger)
    : EfRepository<Inventory>(dbContext), IInventoryRepository
{
    public async Task EnsureInventoryAsync(List<ProcessOrderItemDto> orderItems)
    {
        var itemIds = orderItems.Select(x => x.ItemId).ToList();
        var inventoryRecords = DbContext.Inventories.Where(x => itemIds.Contains(x.ItemId))
            .ToList();

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
                throw new Exception($"Not enough items in inventory for item {item.ItemId}");
            }
        }

        await DbContext.SaveChangesAsync();
    }

    public async Task<bool> DecrementInventoryAsync(int itemId, int amount)
    {
        var inventoryRecord = await GetByIdAsync(itemId);

        if (inventoryRecord == null)
            throw new Exception("Inventory record not found");

        inventoryRecord.AvailableAmount -= amount;
        await DbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> IncrementInventoryAsync(int itemId, int amount)
    {
        var inventoryRecord = await GetByIdAsync(itemId);

        if (inventoryRecord == null)
            throw new Exception("Inventory record not found");

        inventoryRecord.AvailableAmount += amount;
        await UpdateAsync(inventoryRecord);
        return true;
    }

    public async Task<bool> CheckInventoryAsync(int itemId, int amount)
    {
        var inventoryRecord = await GetByIdAsync(itemId);

        if (inventoryRecord == null)
            throw new Exception("Inventory record not found");

        return inventoryRecord.AvailableAmount >= amount;
    }
}