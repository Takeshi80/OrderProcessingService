using Microsoft.Extensions.Logging;
using OPS.Data.Models;

namespace OPS.Data.Repositories;

public interface IItemRepository
{
    Task<bool> DoesItemsExistAsync(List<int> itemId);
}

public class ItemRepository(
    AppDbContext dbContext,
    ILogger<ItemRepository> logger)
    : EfRepository<Item>(dbContext), IItemRepository
{
    public async Task<bool> DoesItemsExistAsync(List<int> itemId)
    {
        var items = await ListAsync(x => itemId.Contains(x.Id));
        var existingId = items.Select(x => x.Id);
        var missingId = itemId.Except(existingId).ToList();

        if (missingId.Count == 0)
            return true;

        logger.LogError($"Following items not found {string.Join(",", missingId)}");
        return false;
    }
}