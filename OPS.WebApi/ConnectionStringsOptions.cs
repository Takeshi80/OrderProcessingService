namespace OPS.WebApi;

public class ConnectionStringsOptions
{
    public string DefaultConnection { get; set; } = null!;
    public string RabbitMq { get; set; } = null!;
}