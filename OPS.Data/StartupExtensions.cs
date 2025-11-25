using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OPS.Data.Interface;
using OPS.Data.Repositories;

namespace OPS.Data;

public static class StartupExtensions
{
    public static void AddOpsDpContext(this IServiceCollection services,
        IConfigurationManager configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));


        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
    }
}