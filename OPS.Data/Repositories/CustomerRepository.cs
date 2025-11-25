using Microsoft.Extensions.Logging;
using OPS.Data.Models;

namespace OPS.Data.Repositories;

public interface ICustomerRepository
{
    Task<Customer?> GetById(int id);
}

public class CustomerRepository(
    AppDbContext dbContext,
    ILogger<CustomerRepository> logger)
    : EfRepository<Customer>(dbContext), ICustomerRepository
{
    public async Task<Customer?> GetById(int id)
    {
        return await GetByIdAsync(id);
    }
}