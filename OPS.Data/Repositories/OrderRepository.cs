using Microsoft.Extensions.Logging;
using OPS.Data.Models;
using OPS.Shared;

namespace OPS.Data.Repositories;

public interface IOrderRepository
{
    Task<Order> CreateNewOrder(ProcessOrderRequestDto dto);
}

public class OrderRepository(AppDbContext dbContext, ILogger<OrderRepository> logger)
    : EfRepository<Order>(dbContext), IOrderRepository
{
    public async Task<Order> CreateNewOrder(ProcessOrderRequestDto dto)
    {
        logger.LogInformation($"Creating new order for customer {dto.CustomerId}");
        
        var order = new Order
        {
            CustomerId = dto.CustomerId,
            Status = OrderStatus.Created,
            TotalAmount = dto.TotalAmount
        };

        foreach (var item in dto.Items)
        {
            var oderItem = new OrderItem
            {
                ItemId = item.ItemId,
                Quantity = item.Quantity
            };
            order.OrderItems.Add(oderItem);
        }

        return await AddAsync(order);
    }
}