using System.ComponentModel.DataAnnotations;
using OPS.Data.Interface;

namespace OPS.Data.Models;

public class Customer : IEntity
{
    [Key] public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    // History of orders
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}