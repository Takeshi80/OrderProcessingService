using Microsoft.Extensions.Logging;
using OPS.Data.Models;

namespace OPS.Data.Repositories;

public interface IItemRepository
{
    Task<bool> EnsureItemExistsAndPopUpPrice(OrderDto dto);
    Task<IReadOnlyList<Item>> List();
}

public class ItemRepository(
    AppDbContext dbContext,
    ILogger<ItemRepository> logger)
    : EfRepository<Item>(dbContext), IItemRepository
{
    public async Task<IReadOnlyList<Item>> List()
    {
        return await ListAsync();
    }

    public async Task<bool> EnsureItemExistsAndPopUpPrice(OrderDto dto)
    {
        var itemIds = dto.Items.Select(x => x.ItemId).ToList();

        var dbItems = await ListAsync(x => itemIds.Contains(x.Id));

        var existingId = dbItems.Select(x => x.Id);
        var missingId = itemIds.Except(existingId).ToList();

        if (missingId.Count == 0)
        {
            foreach (var item in dto.Items)
            {
                var dbItem = dbItems.First(x => x.Id == item.ItemId);
                item.UnitPrice = dbItem.UnitPrice;
            }

            return true;
        }


        logger.LogError($"Following items not found {string.Join(",", missingId)}");
        return false;
    }
}