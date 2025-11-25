using Microsoft.Extensions.Logging;
using OPS.Messages;
using OPS.Processor.Services;
using OPS.Shared;

namespace OPS.Processor.Handlers;

public class SubmitOrderHandler(
    IOrderProcessingService orderProcessingService,
    ILogger<SubmitOrderHandler> logger)
    : IHandleMessages<SubmitOrder>
{
    public async Task Handle(SubmitOrder message, IMessageHandlerContext context)
    {
        logger.LogInformation(
            $"[Order.Processor] Handling SubmitOrder {message.OrderId} for Customer {message.CustomerId}");

        if (message.Items.Count == 0)
        {
            throw new InvalidOperationException("Order must contain at least one item.");
        }

        await orderProcessingService.ProcessOrderAsync(new ProcessOrderRequestDto
        {
            CustomerId = message.CustomerId,
            Items = message.Items.Select(x => new ProcessOrderItemDto
            {
                ItemId = x.ItemId,
                Quantity = x.Quantity
            }).ToList()
        });

        // Publish event for other services TODO: think of it 
        var orderSubmitted = new OrderSubmitted
        {
            OrderId = message.OrderId,
            CustomerId = message.CustomerId,
            SubmittedAtUtc = DateTime.UtcNow
        };

        await context.Publish(orderSubmitted);

        logger.LogInformation($"[Order.Processor] Published OrderSubmitted for {message.OrderId}");
    }
}