namespace OPS.Messages;

public class OrderSubmitted : IEvent
{
    public Guid OrderId { get; set; }
    public int CustomerId { get; set; }
    public DateTime SubmittedAtUtc { get; set; }
}