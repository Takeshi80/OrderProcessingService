using Microsoft.Extensions.Options;
using OPS.Messages;
using OPS.WebApi.Dtos;
using Endpoint = NServiceBus.Endpoint;

namespace OPS.WebApi.RabbitMq;

public interface IRabbitMqSender
{
    Task<Guid> Send(ProcessOrderRequestDto data);
}

public class RabbitMqSender(
    IOptions<ConnectionStringsOptions> connectionStrings,
    ILogger<RabbitMqSender> logger) : IRabbitMqSender
{
    private readonly string _defaultConnection = connectionStrings.Value.RabbitMq;

    public async Task<Guid> Send(ProcessOrderRequestDto data)
    {
        var endpointConfiguration = new EndpointConfiguration("Order.Sender");

        var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
        transport.ConnectionString(_defaultConnection);
        transport.UseConventionalRoutingTopology(QueueType.Classic);

        // Routing: send SubmitOrder commands to Order.Processor
        var routing = transport.Routing();
        routing.RouteToEndpoint(typeof(SubmitOrder), "Order.Processor");

        endpointConfiguration.SendFailedMessagesTo("order.sender.error");
        endpointConfiguration.AuditProcessedMessagesTo("order.sender.audit");
        endpointConfiguration.EnableInstallers();
        endpointConfiguration.UseSerialization<SystemJsonSerializer>();

        var endpointInstance = await Endpoint.Start(endpointConfiguration);
        logger.LogInformation("Order.Sender endpoint started.");

        var orderId = Guid.NewGuid();

        var command = new SubmitOrder
        {
            OrderId = orderId,
            CustomerId = data.CustomerId,
            Items = data.Items.Select(x => new OrderItem()
            {
                ItemId = x.ItemId,
                Quantity = x.Quantity
            }).ToList()
        };

        logger.LogInformation($"Sending SubmitOrder {orderId}...");
        await endpointInstance.Send(command);

        await endpointInstance.Stop();

        return orderId;
    }
}