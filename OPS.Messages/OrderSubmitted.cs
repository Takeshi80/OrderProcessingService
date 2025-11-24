namespace OPS.Messages;

public class OrderSubmitted : IEvent
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public DateTime SubmittedAtUtc { get; set; }
}