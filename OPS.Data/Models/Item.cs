using System.ComponentModel.DataAnnotations;
using OPS.Data.Interface;

namespace OPS.Data.Models;

public class Item: IEntity
{
    [Key]
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    // Order items
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}