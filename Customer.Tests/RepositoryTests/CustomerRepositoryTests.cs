using Customer.Contracts;
using Customer.Database;
using Customer.Repository;
using Microsoft.EntityFrameworkCore;

namespace Customer.Tests.RepositoryTests
{
    public class CustomerRepositoryTests
    {
        [Fact]
        public async void GetByNameSingleMatch()
        {
            ICustomerRepository customerRepository = GetInMemoryPersonRepository();
            var searchResult = await customerRepository.GetByName("Alan");
            Assert.True(searchResult.Count == 1 && searchResult.First().FirstName == "Alan", "There should be only one Alan");
        }

        [Fact]
        public async void GetByNameTwoMatches()
        {
            ICustomerRepository customerRepository = GetInMemoryPersonRepository();
            var searchResult = await customerRepository.GetByName("Smith");
            Assert.True(searchResult.Count == 2 && searchResult.First().LastName == "Smith", "There should be two Smiths");
        }

        [Fact]
        public async void GetByNameNoMatch()
        {
            ICustomerRepository customerRepository = GetInMemoryPersonRepository();
            var searchResult = await customerRepository.GetByName("Ryan");
            Assert.False(searchResult.Count > 1, "There should not be a Ryan");
        }

        [Fact]
        public async void GetByNameNullValue()
        {
            ICustomerRepository customerRepository = GetInMemoryPersonRepository();
            var searchResult = await customerRepository.GetByName(null);
            Assert.True(searchResult != null &&  searchResult.Count == 0, "Empty list should be returned on null value search");
        }

        [Fact]
        public async void GetByIdMatch()
        {
            ICustomerRepository customerRepository = GetInMemoryPersonRepository();
            CustomerDto retrievedCustomer = await customerRepository.GetById(7);
            Assert.True(retrievedCustomer != null && retrievedCustomer.Id == 7, "Single match with Id = 7 expected");
        }

        [Fact]
        public async void GetByIdNoMatch()
        {
            ICustomerRepository customerRepository = GetInMemoryPersonRepository();
            CustomerDto? retrievedCustomer = await customerRepository.GetById(9);
            Assert.True(retrievedCustomer == null, "A null value should be returned");
        }

        [Fact]
        public async void CreateValid()
        {
            ICustomerRepository customerRepository = GetInMemoryPersonRepository();
            CustomerDto validCustomer = new CustomerDto() { Id = 0, DateOfBirth = new DateTime(1983, 1, 1), FirstName = "Bill", LastName = "Bob" };
            CustomerDto? createdCustomer = await customerRepository.Create(validCustomer);
            Assert.True(createdCustomer != null && createdCustomer.Id == 8);
        }

        [Fact]
        public async void CreateIdNotZero()
        {
            // Id must be 0
            ICustomerRepository customerRepository = GetInMemoryPersonRepository();
            CustomerDto invalidCustomer = new CustomerDto() { Id = 5, DateOfBirth = new DateTime(1983, 1, 1), FirstName = "Bill", LastName = "Bob" };
            var createdCustomer = await customerRepository.Create(invalidCustomer);
            Assert.Null(createdCustomer);
        }

        [Fact]
        public async void CreateMissingFields()
        {
            // Database allows empty text fields
            ICustomerRepository customerRepository = GetInMemoryPersonRepository();
            var invalidCustomer = new CustomerDto() { Id = 0, DateOfBirth = new DateTime(1983, 1, 1), FirstName = "", LastName = "" };
            var createdCustomer = await customerRepository.Create(invalidCustomer);
            Assert.NotNull(createdCustomer);
        }

        [Fact]
        public async void UpdateValidCustomer()
        {
            ICustomerRepository customerRepository = GetInMemoryPersonRepository();
            CustomerDto updatedCustomer = new CustomerDto() { Id = 1, DateOfBirth = new DateTime(1983, 1, 1), FirstName = "Alan", LastName = "Smith" };
            var result = await customerRepository.Update(updatedCustomer);
            Assert.True(result, "Update should occur");
        }

        [Fact]
        public async void UpdateMissingCustomerFields()
        {
            ICustomerRepository customerRepository = GetInMemoryPersonRepository();
            CustomerDto updatedCustomer = new CustomerDto() { Id = 1, DateOfBirth = new DateTime(1983, 1, 1), FirstName = "", LastName = "" };
            var result = await customerRepository.Update(updatedCustomer);
            Assert.True(result, "Database allows empty text fields");
        }

        [Fact]
        public async void UpdateInvalidCustomerId()
        {
            ICustomerRepository customerRepository = GetInMemoryPersonRepository();
            CustomerDto updatedCustomer = new CustomerDto() { Id = 8, DateOfBirth = new DateTime(1983, 1, 1), FirstName = "Bill", LastName = "Bob" };
            var result = await customerRepository.Update(updatedCustomer);
            Assert.False(result, "Customer with that Id does not exist cannot be updated");
        }

        [Fact]
        public async void DeleteValidCustomerId()
        {
            ICustomerRepository customerRepository = GetInMemoryPersonRepository();
            var result = await customerRepository.Delete(7);
            Assert.True(result, "Repository returns true indicating delete occurred");
        }

        [Fact]
        public async void DeleteInvalidCustomerId()
        {
            ICustomerRepository customerRepository = GetInMemoryPersonRepository();
            var result = await customerRepository.Delete(8);
            Assert.False(result, "Repository returns true indicating delete failed");
        }

        private ICustomerRepository GetInMemoryPersonRepository()
        {
            DbContextOptions<CustomerContext> options;
            var builder = new DbContextOptionsBuilder<CustomerContext>();
            builder.UseInMemoryDatabase("TestCustomerDatabase");
            options = builder.Options;
            CustomerContext personDataContext = new CustomerContext(options);
            personDataContext.Database.EnsureDeleted();
            personDataContext.Database.EnsureCreated();

            personDataContext.Add(new CustomerDto() { Id = 1, DateOfBirth = new DateTime(2000, 1, 1), FirstName = "Alan", LastName = "Smith" });
            personDataContext.Add(new CustomerDto() { Id = 2, DateOfBirth = new DateTime(1980, 1, 1), FirstName = "Anne", LastName = "Green" });
            personDataContext.Add(new CustomerDto() { Id = 3, DateOfBirth = new DateTime(1956, 1, 1), FirstName = "Bob", LastName = "Black" });
            personDataContext.Add(new CustomerDto() { Id = 4, DateOfBirth = new DateTime(1970, 1, 1), FirstName = "William", LastName = "Hargrave" });
            personDataContext.Add(new CustomerDto() { Id = 5, DateOfBirth = new DateTime(2005, 1, 1), FirstName = "Selina", LastName = "Kyle" });
            personDataContext.Add(new CustomerDto() { Id = 6, DateOfBirth = new DateTime(1978, 1, 1), FirstName = "Alice", LastName = "Hill" });
            personDataContext.Add(new CustomerDto() { Id = 7, DateOfBirth = new DateTime(1986, 1, 1), FirstName = "Heath", LastName = "Smith" });
            personDataContext.SaveChanges();

            return new CustomerRepository(personDataContext);
        }
    }
}