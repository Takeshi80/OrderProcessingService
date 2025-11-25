using Microsoft.Extensions.Logging;
using OPS.Data;
using OPS.Data.Repositories;
using OPS.Shared;

namespace OPS.Processor.Services;

public interface IOrderProcessingService
{
    public Task ProcessOrderAsync(ProcessOrderRequestDto dto);
}

public class OrderProcessingService(
    IOrderRepository orderRepository,
    ICustomerRepository customerRepository,
    ILogger<OrderProcessingService> logger,
    AppDbContext dbContext) : IOrderProcessingService
{
    public async Task ProcessOrderAsync(ProcessOrderRequestDto dto)
    {
        try
        {
            await using var ctx = await dbContext.Database.BeginTransactionAsync();

            // TODO: make it better
            var customer = await customerRepository.EnsureCustomerExists("mock1", "mock2");
            dto.CustomerId = customer.Id;
            await orderRepository.CreateNewOrder(dto);
            
            await ctx.CommitAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing order");
        }
    }
}