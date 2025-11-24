using OPS.Messages;
using Endpoint = NServiceBus.Endpoint;

namespace OPS.WebApi.RabbitMq;

public interface IRabbitMqSender
{
    Task Send();
}

public class RabbitMqSender: IRabbitMqSender
{
    public async Task Send()
    {
        var endpointConfiguration = new EndpointConfiguration("Order.Sender");

        var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
        transport.ConnectionString("host=rabbitmq;username=appuser;password=appsecret");
        transport.UseConventionalRoutingTopology(QueueType.Classic);

        // Routing: send SubmitOrder commands to Order.Processor
        var routing = transport.Routing();
        routing.RouteToEndpoint(typeof(SubmitOrder), "Order.Processor");

        endpointConfiguration.SendFailedMessagesTo("order.sender.error");
        endpointConfiguration.AuditProcessedMessagesTo("order.sender.audit");
        endpointConfiguration.EnableInstallers();
        endpointConfiguration.UseSerialization<SystemJsonSerializer>();

        var endpointInstance = await Endpoint.Start(endpointConfiguration);
        Console.WriteLine("Order.Sender endpoint started.");

        var orderId = 1;
        var customerId = 2;

        var command = new SubmitOrder
        {
            OrderId = orderId,
            CustomerId = customerId,
            Items =
            {
                new OrderItem() { ItemId = 1, Quantity = 2 },
                new OrderItem() { ItemId = 2, Quantity = 1 }
            }
        };

        Console.WriteLine($"Sending SubmitOrder {orderId}...");
        await endpointInstance.Send(command);

        await endpointInstance.Stop();
    }
}