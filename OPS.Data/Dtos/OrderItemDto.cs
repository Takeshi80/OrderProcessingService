namespace OPS.Data;

public record OrderDto
{
    public Guid OrderId { get; set; }
    public int CustomerId { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
    
    public int TotalAmount => Items.Sum(x => x.Quantity);
}

public record OrderItemDto
{
    public int ItemId { get; set; }
    public int Quantity { get; set; }
}