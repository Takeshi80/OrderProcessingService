using OPS.Messages;

namespace OPS.Processor;

public class SubmitOrderHandler : IHandleMessages<SubmitOrder>
{
    public async Task Handle(SubmitOrder message, IMessageHandlerContext context)
    {
        Console.WriteLine($"[Order.Processor] Handling SubmitOrder {message.OrderId} for Customer {message.CustomerId}");

        if (message.Items.Count == 0)
        {
            throw new InvalidOperationException("Order must contain at least one item.");
        }

        // TODO: Persist order in DB, etc.

        // Publish event for other services TODO: think of it 
        var orderSubmitted = new OrderSubmitted
        {
            OrderId = message.OrderId,
            CustomerId = message.CustomerId,
            SubmittedAtUtc = DateTime.UtcNow
        };

        await context.Publish(orderSubmitted);

        Console.WriteLine($"[Order.Processor] Published OrderSubmitted for {message.OrderId}");
    }
}