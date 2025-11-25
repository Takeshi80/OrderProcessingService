using System.ComponentModel.DataAnnotations;
using OPS.Data.Interface;

namespace OPS.Data.Models;

public class Order : IEntity
{
    [Key] public Guid Id { get; set; }

    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public int TotalAmount { get; set; }

    public OrderStatus Status { get; set; }


    // Order items
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}