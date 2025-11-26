using OPS.Data.Models;

namespace OPS.WebApi.Dtos;

public class OrderResponseDto
{
    public Guid OrderId { get; set; }
    public string? CustomerFullName { get; set; }
    public int TotalItemsAmount { get; set; }
    public decimal PriceWithoutDiscount { get; set; }
    public decimal PriceWithDiscount { get; set; }
    public List<OrderItemResponseDto>? Items { get; set; } = new();
    public string? FailureReason { get; set; }

    public static OrderResponseDto FromOrder(Order order)
    {
        return new OrderResponseDto
        {
            OrderId = order.Id,
            CustomerFullName = order.Customer == null
                ? string.Empty
                : $"{order.Customer.FirstName} {order.Customer.LastName}",
            TotalItemsAmount = order.OrderItems?.Sum(x => x.Quantity) ?? 0,
            PriceWithoutDiscount = order.PriceWithoutDiscount,
            PriceWithDiscount = order.PriceWithDiscount,
            FailureReason = order.FailureReason,
            Items = order.OrderItems?.Select(x => new OrderItemResponseDto
            {
                Id = x.ItemId,
                Name = x.Item.Name,
                Quantity = x.Quantity,
                UnitPrice = x.Item.UnitPrice
            }).ToList()
        };
    }
}

public class OrderItemResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}