namespace OPS.Messages;

public class SubmitOrder : ICommand
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public List<OrderItem> Items { get; set; } = new();
}

public class OrderItem
{
    public int ItemId { get; set; }
    public int Quantity { get; set; }
}