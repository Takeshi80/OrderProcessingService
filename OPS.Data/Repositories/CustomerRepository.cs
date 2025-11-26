using Microsoft.Extensions.Logging;
using OPS.Data.Models;

namespace OPS.Data.Repositories;

public interface ICustomerRepository
{
    Task<Customer?> GetById(int id);
    Task<IReadOnlyList<Customer>> List();
}

public class CustomerRepository(
    AppDbContext dbContext,
    ILogger<CustomerRepository> logger)
    : EfRepository<Customer>(dbContext), ICustomerRepository
{
    public async Task<IReadOnlyList<Customer>> List()
    {
        return await ListAsync();
    }

    public async Task<Customer?> GetById(int id)
    {
        return await GetByIdAsync(id);
    }
}