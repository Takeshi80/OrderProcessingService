using System.ComponentModel.DataAnnotations;
using OPS.WebApi.Data.Interface;

namespace OPS.WebApi.Data.Models;

public class Item: IEntity
{
    [Key]
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    // Order items
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}