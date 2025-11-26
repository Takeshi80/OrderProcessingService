namespace OPS.Data;

public record OrderDto
{
    public Guid OrderId { get; set; }
    public int? CustomerId { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();

    public int TotalAmount => Items.Sum(x => x.Quantity);
    public decimal PriceWithoutDiscount => Items.Sum(x => x.UnitPrice * x.Quantity);
    public decimal PriceWithDiscount { get; set; }
    public string? FailureReason { get; set; }
}

public record OrderItemDto
{
    public int ItemId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}