using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OPS.Data.Models;

namespace OPS.Data.Repositories;

public interface IOrderRepository
{
    Task<Order> CreateNewOrder(OrderDto dto, OrderStatus status);

    Task<Order?> GetById(Guid id);
}

public class OrderRepository(AppDbContext dbContext, ILogger<OrderRepository> logger)
    : EfRepository<Order>(dbContext), IOrderRepository
{
    public async Task<Order> CreateNewOrder(OrderDto dto, OrderStatus status)
    {
        logger.LogInformation("Creating new order for customer {customerId}", dto.CustomerId);

        var order = new Order
        {
            Id = dto.OrderId,
            CustomerId = dto.CustomerId,
            Status = status,
            TotalAmount = dto.TotalAmount,
            PriceWithoutDiscount = dto.PriceWithoutDiscount,
            PriceWithDiscount = dto.PriceWithDiscount,
            FailureReason = dto.FailureReason
        };

        if (dto.Items.Any())
        {
            foreach (var oderItem in dto.Items.Select(item => new OrderItem
                     {
                         ItemId = item.ItemId,
                         Quantity = item.Quantity,
                         UnitPrice = item.UnitPrice
                     }))
            {
                order.OrderItems.Add(oderItem);
            }
        }

        return await AddAsync(order);
    }

    public async Task<Order?> GetById(Guid id)
    {
        return await DbContext.Set<Order>()
            .Include(x => x.Customer)
            .Include(x => x.OrderItems).ThenInclude(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}