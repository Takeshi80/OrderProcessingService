using System.ComponentModel.DataAnnotations;
using OPS.Data.Interface;

namespace OPS.Data.Models;

public class Inventory : IEntity
{
    [Key] public int Id { get; set; }

    public int ItemId { get; set; }
    
    public Item Item { get; set; }

    public int AvailableAmount { get; set; }
}