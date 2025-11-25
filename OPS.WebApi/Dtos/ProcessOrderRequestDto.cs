namespace OPS.WebApi.Dtos;

public record ProcessOrderRequestDto
{
    public Guid IdempotencyKey { get; set; }
    public int CustomerId { get; set; }
    public List<ProcessOrderItemDto> Items { get; set; } = new();
    
    public int TotalAmount => Items.Sum(x => x.Quantity);
}

public record ProcessOrderItemDto
{
    public int ItemId { get; set; }
    public int Quantity { get; set; }
}