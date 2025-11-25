using OPS.Data.Interface;

namespace OPS.Data.Models;

public class OrderItem : IEntity
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public int ItemId { get; set; }
    public Item Item { get; set; } = null!;


    public int Quantity { get; set; }
}