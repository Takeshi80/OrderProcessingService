using Microsoft.Extensions.Logging;
using OPS.Data;
using OPS.Data.Repositories;
using OPS.Shared;

namespace OPS.Processor.Services;

public interface IOrderProcessingService
{
    public Task ProcessOrderAsync(ProcessOrderRequestDto dto);
}

public class OrderProcessingService(
    IOrderRepository orderRepository,
    ICustomerRepository customerRepository,
    ILogger<OrderProcessingService> logger,
    IItemRepository itemRepository,
    IInventoryRepository inventoryRepository,
    AppDbContext dbContext) : IOrderProcessingService
{
    public async Task ProcessOrderAsync(ProcessOrderRequestDto dto)
    {
        try
        {
            await using var ctx = await dbContext.Database.BeginTransactionAsync();

            var customer = await customerRepository.GetById(dto.CustomerId);
            if (customer == null)
            {
                throw new Exception("Customer not found");
            }

            var itemIds = dto.Items.Select(x => x.ItemId).ToList();
            var itemExists = await itemRepository.DoesItemsExistAsync(itemIds);

            if (!itemExists)
            {
                throw new Exception("One or more items not found");
            }

            await inventoryRepository.EnsureInventoryAsync(dto.Items);

            await orderRepository.CreateNewOrder(dto);

            await ctx.CommitAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing order");
        }
    }
}