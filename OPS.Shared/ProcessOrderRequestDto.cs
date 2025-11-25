namespace OPS.Shared;

public record ProcessOrderRequestDto
{
    public Guid OrderId { get; set; }
    public int CustomerId { get; set; }
    public List<ProcessOrderItemDto> Items { get; set; } = new();
    public int TotalAmount => Items.Sum(x => x.Quantity);
}

public record ProcessOrderItemDto
{
    public int ItemId { get; set; }
    public int Quantity { get; set; }
}