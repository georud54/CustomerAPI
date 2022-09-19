using Customer.Contracts;
using Customer.Repository;
using Microsoft.Extensions.Logging;

namespace Customer.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<List<CustomerDto>> GetByName(string searchTerm)
        {
            return await _customerRepository.GetByName(searchTerm);
        }

        public async Task<CustomerDto?> GetById(int id)
        {
            return await _customerRepository.GetById(id);
        }

        public async Task<CustomerDto?> Create(CustomerDto newCustomer)
        {
            // Validating not an existing record and all fields have a value
            if (!CheckFields(newCustomer, true))
            {
                return null;
            }

            // Validating Id not already used
            var existingCustomer = await GetById(newCustomer.Id);
            if (existingCustomer != null)
            {
                return null;
            }

            return await _customerRepository.Create(newCustomer);
        }

        public async Task<bool> Update(CustomerDto customer)
        {
            // Validating all fields have a value
            if (!CheckFields(customer, false))
            {
                return false;
            }

            // Validating customer already exists
            var existingCustomer = await GetById(customer.Id);
            if (existingCustomer == null)
            {
                return false;
            }

            return await _customerRepository.Update(customer);
        }

        public async Task<bool> Delete(int id)
        {
            return await _customerRepository.Delete(id);
        }

        private bool CheckFields(CustomerDto record, bool isNewRecord)
        {
            if (record.Id == 0 && !isNewRecord || record.Id != 0 && isNewRecord)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(record.FirstName) || string.IsNullOrWhiteSpace(record.LastName))
            {
                return false;
            }

            return true;
        }
    }
}
