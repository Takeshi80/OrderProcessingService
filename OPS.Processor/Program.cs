using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace OPS.Processor;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.Logging.ClearProviders();
        builder.Logging.AddSimpleConsole(options =>
        {
            options.SingleLine = true;
            options.TimestampFormat = "HH:mm:ss ";
        });

        var endpointConfiguration = GetEndpointConfiguration();

        builder.UseNServiceBus(endpointConfiguration);

        var host = builder.Build();
        await host.RunAsync();
    }

    private static EndpointConfiguration GetEndpointConfiguration()
    {
        var endpointConfiguration = new EndpointConfiguration("Order.Processor");

        var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
        Console.WriteLine("Connecting to RabbitMQ...");
        transport.ConnectionString("host=rabbitmq;username=appuser;password=appsecret");
        transport.UseConventionalRoutingTopology(QueueType.Classic);

        endpointConfiguration.SendFailedMessagesTo("order.processor.error");

        endpointConfiguration.AuditProcessedMessagesTo("order.processor.audit");

        endpointConfiguration.EnableInstallers();

        endpointConfiguration.UseSerialization<SystemJsonSerializer>();

        var recoverability = endpointConfiguration.Recoverability();
        recoverability.Immediate(immediate =>
        {
            immediate.NumberOfRetries(3); // immediate retries before going to delayed or DLQ
        });
        return endpointConfiguration;
    }
}