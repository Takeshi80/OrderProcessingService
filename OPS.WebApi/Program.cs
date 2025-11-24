using Microsoft.EntityFrameworkCore;
using OPS.WebApi;
using OPS.WebApi.Data;
using OPS.WebApi.Data.Interface;
using OPS.WebApi.Data.Repositories;
using OPS.WebApi.Dtos;
using OPS.WebApi.RabbitMq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IRabbitMqSender, RabbitMqSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/order", async (ProcessOrderRequestDto data, IRabbitMqSender sender) =>
    {
        await sender.Send();
        return Results.Ok();
    })
    .WithName("SubmitOrder")
    .WithOpenApi();

app.Run();