using Customer.Contracts;
using Customer.Database;
using Microsoft.EntityFrameworkCore;

namespace Customer.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly CustomerContext _customerContext;

        public CustomerRepository(CustomerContext customerContext)
        {
            _customerContext = customerContext;
        }

        public async Task<List<CustomerDto>> GetByName(string searchTerm)
        {
            if (searchTerm == null)
            {
                return new List<CustomerDto>();
            }

            return await _customerContext.Customers
                .Where(cust => cust.FirstName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) || cust.LastName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToListAsync();
        }

        public async Task<CustomerDto> GetById(int id)
        {
            return await _customerContext.Customers.Where(c => c.Id == id).FirstOrDefaultAsync();
        }

        public async Task<CustomerDto> Create(CustomerDto newCustomer)
        {
            if (newCustomer.Id != 0 || _customerContext.Customers.Any(c => c.Id == newCustomer.Id))
            {
                return null;
            }

            // Getting the current max Id. Obviously not thread safe
            int currentMaxId = _customerContext.Customers.Max(c => c.Id as int?) ?? 0;
            newCustomer.Id = currentMaxId + 1;
            _customerContext.Customers.Add(newCustomer);
            await _customerContext.SaveChangesAsync();

            return newCustomer;
        }

        public async Task<bool> Update(CustomerDto customer)
        {
            var existingRecord = await _customerContext.Customers.Where(c => c.Id == customer.Id).FirstOrDefaultAsync();
            if (existingRecord != null)
            {
                existingRecord.FirstName = customer.FirstName;
                existingRecord.LastName = customer.LastName;
                existingRecord.DateOfBirth = customer.DateOfBirth;
                await _customerContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> Delete(int id)
        {
            var customer = _customerContext.Customers.FirstOrDefault(cust => cust.Id == id);
            if (customer != null)
            {
                _customerContext.Customers.Remove(customer);
                await _customerContext.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}