using OrderProcessingService.Data.Models;

namespace OrderProcessingService.Data.Repositories;

public interface IInventoryRepository
{
    Task<bool> DecrementInventoryAsync(int itemId, int amount);
    Task<bool> IncrementInventoryAsync(int itemId, int amount);
    Task<bool> CheckInventoryAsync(int itemId, int amount);
}

public class InventoryRepository(AppDbContext dbContext) : EfRepository<Inventory>(dbContext), IInventoryRepository
{
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