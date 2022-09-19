using Customer.Contracts;

namespace Customer.Services
{
    public interface ICustomerService
    {
        public  Task<List<CustomerDto>> GetByName(string searchTerm);
        public Task<CustomerDto> GetById(int id);
        public Task<CustomerDto> Create(CustomerDto newCustomer);
        public Task<bool> Update(CustomerDto customer);
        public Task<bool> Delete(int id);
    }
}
