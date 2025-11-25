using OPS.Data;
using OPS.Shared;
using OPS.WebApi;
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/order", async (ProcessOrderRequestDto data, IRabbitMqSender sender) =>
    {
        var orderId = await sender.Send(data);
        return Results.Created($"/order", orderId);
    })
    .WithName("SubmitOrder")
    .WithOpenApi();

app.Run();