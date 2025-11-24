using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OPS.WebApi.Data.Interface;

namespace OPS.WebApi.Data.Models;

public class Inventory : IEntity
{
    [Key] public int Id { get; set; }

    [ForeignKey(nameof(Item))] public int ItemId { get; set; }
    public Item Item { get; set; }

    public int AvailableAmount { get; set; }
}