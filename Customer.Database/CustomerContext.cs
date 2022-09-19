using Customer.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Customer.Database
{
    public class CustomerContext : DbContext
    {
        public CustomerContext(DbContextOptions<CustomerContext> options)
            : base(options)
        {
            
        }

        public DbSet<CustomerDto> Customers { get; set; } = null!;
    }
}