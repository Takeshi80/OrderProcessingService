using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OPS.Data.Models;

namespace OPS.Data.Repositories;

public interface IOrderRepository
{
    Task<Order> CreateNewOrder(OrderDto dto);

    Task<Order?> GetById(Guid id);
}

public class OrderRepository(AppDbContext dbContext, ILogger<OrderRepository> logger)
    : EfRepository<Order>(dbContext), IOrderRepository
{
    public async Task<Order> CreateNewOrder(OrderDto dto)
    {
        logger.LogInformation("Creating new order for customer {customerId}", dto.CustomerId);

        var order = new Order
        {
            Id = dto.OrderId,
            CustomerId = dto.CustomerId,
            Status = OrderStatus.Created,
            TotalAmount = dto.TotalAmount
        };

        foreach (var oderItem in dto.Items.Select(item => new OrderItem
                 {
                     ItemId = item.ItemId,
                     Quantity = item.Quantity
                 }))
        {
            order.OrderItems.Add(oderItem);
        }

        return await AddAsync(order);
    }

    public async Task<Order?> GetById(Guid id)
    {
        return await DbContext.Set<Order>()
            .Include(x => x.OrderItems)
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}