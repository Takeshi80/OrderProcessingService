using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OrderProcessingService.Data.Interface;

namespace OrderProcessingService.Data.Models;

public class Order : IEntity
{
    [Key] public int Id { get; set; }

    [ForeignKey(nameof(Customer))] public int CustomerId { get; set; }
    public Customer Customer { get; set; }

    public int TotalAmount { get; set; }

    public OrderStatus Status { get; set; }


    // Order items
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}