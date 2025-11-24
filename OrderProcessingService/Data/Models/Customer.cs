using System.ComponentModel.DataAnnotations;
using OrderProcessingService.Data.Interface;

namespace OrderProcessingService.Data.Models;

public class Customer : IEntity
{
    [Key] public int Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    // History of orders
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}