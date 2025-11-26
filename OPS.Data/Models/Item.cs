using System.ComponentModel.DataAnnotations;
using OPS.Data.Interface;

namespace OPS.Data.Models;

public class Item : IEntity
{
    [Key] public int Id { get; set; }

    [MaxLength(256)]
    public string Name { get; set; } = null!;

    public decimal UnitPrice { get; set; }

    // Order items
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}