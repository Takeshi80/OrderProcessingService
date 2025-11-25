using Microsoft.EntityFrameworkCore;
using OPS.Data;
using OPS.Data.Models;
using OPS.Data.Repositories;
using OPS.WebApi;
using OPS.WebApi.Dtos;
using OPS.WebApi.RabbitMq;
using OPS.WebApi.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// register dependencies for DB
builder.Services.AddOpsDpContext(builder.Configuration);

builder.Services.Configure<ConnectionStringsOptions>(builder.Configuration.GetSection("ConnectionStrings"));

builder.Services.AddScoped<IRabbitMqSender, RabbitMqSender>();
Console.WriteLine("CS " + builder.Configuration.GetConnectionString("DefaultConnection"));
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbInitializer.Seed(context);
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapPost("/order", async (ProcessOrderRequestDto data, IRabbitMqSender sender, AppDbContext db) =>
    {
        var clientId = "web-api";
        var key = data.IdempotencyKey;
        var entry = new IdempotentRequest
        {
            Id = Guid.NewGuid(),
            ClientId = clientId,
            IdempotencyKey = data.IdempotencyKey,
            CreatedAt = DateTime.UtcNow,
        };

        try
        {
            db.IdempotentRequests.Add(entry);
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            var existing = await db.IdempotentRequests
                .FirstOrDefaultAsync(x => x.ClientId == clientId && x.IdempotencyKey == key);

            if (existing?.OrderId is not null)
            {
                return Results.Created($"/order", existing.OrderId);
            }

            return Results.StatusCode(StatusCodes.Status409Conflict);
        }

        var orderId = await sender.Send(data);

        // store order id for idempotency request
        entry.OrderId = orderId;
        await db.SaveChangesAsync();

        return Results.Created($"/order", orderId);
    })
    .WithName("SubmitOrder")
    .WithOpenApi();

app.MapGet("/order/{orderId}", async (Guid orderId, IOrderRepository orderRepository) =>
{
    var order = await orderRepository.GetById(orderId);

    return order == null
        ? Results.NotFound()
        : Results.Ok(order);
});

app.Run();