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
            using var scope = logger.BeginScope("OrderId:{OrderId}", dto.OrderId);
            var status = OrderStatus.Created;

            // cannot run it in parallel with current EF config, but can be optimized in future
            if (await CustomerValidation(dto) == false
                | await ItemsValidation(dto) == false)

            {
                status = OrderStatus.Failed;
            }

            // discount logic if > 3 items 10% discount 
            if (dto.TotalAmount > 3)
            {
                dto.PriceWithDiscount = dto.PriceWithoutDiscount * 0.9m;
                logger.LogInformation("Applied 10% discount, new price {price}", dto.PriceWithDiscount);
            }

            var order = await orderRepository.CreateNewOrder(dto, status);

            // Skip any further processing if order validation failed
            if (status == OrderStatus.Failed)
            {
                return;
            }

            order.Status = OrderStatus.Processed;

            try
            {
                // Decrementing inventory
                await inventoryRepository.EnsureInventoryAsync(dto.Items);
            }
            catch (NotEnoughInventoryException ex)
            {
                order.FailureReason = "Not enough inventory";
                logger.LogError(ex, "Not enough inventory for order {OrderId}", order.Id);
                order.Status = OrderStatus.Failed;
            }
            catch (Exception ex)
            {
                order.FailureReason = "Unexpected error during inventory update";
                logger.LogError(ex, "Unexpected exception for order {OrderId}", order.Id);
                order.Status = OrderStatus.Failed;
            }

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

    private async Task<bool> ItemsValidation(OrderDto dto)
    {
        var itemExists = await itemRepository.EnsureItemExistsAndPopUpPrice(dto);

        if (itemExists)
            return true;

        dto.FailureReason = "One or more items not found in DB";
        logger.LogError("One or more items not found");

        dto.Items = new List<OrderItemDto>(); // not the best practice, but it's just a demo'
        return false;
    }

    private async Task<bool> CustomerValidation(OrderDto dto)
    {
        // Customer validation
        if (dto.CustomerId is null)
        {
            dto.FailureReason = "No customer provided.";
            logger.LogError("No customer provided");
            return false;
        }

        var customer = await customerRepository.GetById(dto.CustomerId.Value);
        if (customer != null)
            return true;

        dto.CustomerId = null; // not the best practice, but it's just a demo'
        dto.FailureReason = "No customer found in DB";
        logger.LogError("Unable to find customer reference in DB");
        return false;
    }
}