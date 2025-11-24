using System.ComponentModel.DataAnnotations.Schema;
using OPS.Data.Interface;

namespace OPS.Data.Models;

public class OrderItem : IEntity
{
    [ForeignKey(nameof(Order))] public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    [ForeignKey(nameof(Item))] public int ItemId { get; set; }

    public Item Item { get; set; } = null!;


    public int Quantity { get; set; }
}