using Microsoft.Extensions.Logging;
using OPS.Data;
using OPS.Data.Models;
using OPS.Data.Repositories;

namespace OPS.Processor.Services;

public interface IOrderProcessingService
{
    public Task ProcessOrderAsync(OrderDto dto);
}

public class OrderProcessingService(
    IOrderRepository orderRepository,
    ICustomerRepository customerRepository,
    ILogger<OrderProcessingService> logger,
    IItemRepository itemRepository,
    IInventoryRepository inventoryRepository,
    AppDbContext dbContext) : IOrderProcessingService
{
    private static long _processedOrdersCount = 0;

    public async Task ProcessOrderAsync(OrderDto dto)
    {
        try
        {
            // Customer validation
            var customer = await customerRepository.GetById(dto.CustomerId);
            if (customer == null)
            {
                throw new Exception("Customer not found");
            }

            // items validation
            var itemIds = dto.Items.Select(x => x.ItemId).ToList();
            var itemExists = await itemRepository.DoesItemsExistAsync(itemIds);

            if (!itemExists)
            {
                throw new Exception("One or more items not found");
            }

            // If validation pass -> we end up with order with status "CREATED"
            var order = await orderRepository.CreateNewOrder(dto);

            order.Status = OrderStatus.Processed;
            try
            {
                // Decrementing inventory
                await inventoryRepository.EnsureInventoryAsync(dto.Items);
            }
            catch (NotEnoughInventoryException ex)
            {
                logger.LogError(ex, "Not enough inventory for order {OrderId}", order.Id);
                order.Status = OrderStatus.Failed;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected exception for order {OrderId}", order.Id);
                order.Status = OrderStatus.Failed;
            }

            // TODO: any other business logic

            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing order");
            throw;
        }

        var total = Interlocked.Increment(ref _processedOrdersCount);
        logger.LogInformation(
            "Processed order {OrderId}. Total processed orders in this instance: {TotalProcessed}",
            dto.OrderId,
            total);
    }
}