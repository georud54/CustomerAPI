using Customer.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.Repository
{
    public interface ICustomerRepository
    {
        public Task<List<CustomerDto>> GetByName(string searchTerm);
        public Task<CustomerDto> GetById(int id);
        public Task<CustomerDto> Create(CustomerDto newCustomer);
        public Task<bool> Update(CustomerDto customer);
        public Task<bool> Delete(int id);
    }
}
