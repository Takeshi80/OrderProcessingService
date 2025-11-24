using System.ComponentModel.DataAnnotations;
using OrderProcessingService.Data.Interface;

namespace OrderProcessingService.Data.Models;

public class Item: IEntity
{
    [Key]
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    // Order items
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}