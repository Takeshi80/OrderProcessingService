using Microsoft.Extensions.Logging;
using OPS.Data.Models;

namespace OPS.Data.Repositories;

public interface ICustomerRepository
{
    Task<Customer> EnsureCustomerExists(string firstName, string lastName);
}

public class CustomerRepository(
    AppDbContext dbContext,
    ILogger<CustomerRepository> logger)
    : EfRepository<Customer>(dbContext), ICustomerRepository
{
    public async Task<Customer> EnsureCustomerExists(string firstName, string lastName)
    {
        // TODO: make it better
        var customer = DbContext.Set<Customer>()
            .FirstOrDefault(c => c.FirstName == firstName && c.LastName == lastName);

        if (customer != null)
        {
            logger.LogInformation("Customer {firstName} {lastName} already exists", firstName, lastName);
            return customer;
        }

        logger.LogInformation("Creating a new customer {firstName} {lastName}", firstName, lastName);
        customer = new Customer { FirstName = firstName, LastName = lastName };
        return await AddAsync(customer);
    }
}