using System.ComponentModel.DataAnnotations;

namespace OPS.Data.Models;

public class IdempotentRequest
{
    public Guid Id { get; set; }
    
    [MaxLength(256)] public string ClientId { get; set; } = null!;
    
    public Guid IdempotencyKey { get; set; }
    
    public Guid? OrderId { get; set; }
    
    public DateTime CreatedAt { get; set; }
}