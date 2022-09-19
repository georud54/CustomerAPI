using Customer.Contracts;
using Customer.Repository;
using Customer.Services;
using Moq;

namespace Customer.Tests.ServiceTests
{
    public class CustomerServiceTests
    {
        private Mock<ICustomerRepository> _customerRepository;
        private readonly ICustomerService _customerService;


        public CustomerServiceTests()
        {
            _customerRepository = new Mock<ICustomerRepository>();
            _customerService = new CustomerService(_customerRepository.Object);
        }

        [Fact]
        public async void GetByNameSingleMatch()
        {
            string searchTerm = "Alan";
            var customerData = GetStandardTestData();
            _customerRepository.Setup(p => p.GetByName(searchTerm)).Returns(Task.FromResult(customerData.Where(c => c.FirstName == searchTerm).ToList()));
            var result = await _customerService.GetByName(searchTerm);
            Assert.True(result.Count == 1 && result.First().FirstName == "Alan", "There should be only one Alan");
        }

        [Fact]
        public async void GetByNameTwoMatches()
        {
            string searchTerm = "Smith";
            var customerData = GetStandardTestData();
            _customerRepository.Setup(p => p.GetByName(searchTerm)).Returns(Task.FromResult(customerData.Where(c => c.LastName == searchTerm).ToList()));
            var result = await _customerService.GetByName(searchTerm);
            Assert.True(result.Count == 2 && result.First().LastName == "Smith", "There should be two Smiths");
        }

        [Fact]
        public async void GetByNameNoMatch()
        {
            string searchTerm = "Ryan";
            var customerData = GetStandardTestData();
            _customerRepository.Setup(p => p.GetByName(searchTerm)).Returns(Task.FromResult(customerData.Where(c => c.LastName == searchTerm).ToList()));
            var result = await _customerService.GetByName(searchTerm);
            Assert.False(result.Count > 0, "There should not be a Ryan");
        }

        [Fact]
        public async void GetByNameNullValue()
        {
            string searchTerm = null;
            var customerData = GetStandardTestData();
            _customerRepository.Setup(p => p.GetByName(searchTerm)).Returns(Task.FromResult(customerData.Where(c => c.LastName == searchTerm).ToList()));
            var result = await _customerService.GetByName(searchTerm);
            Assert.False(result.Count > 0, "Empty list should be returned on null value search");
        }

        [Fact]
        public async void GetByIdMatch()
        {
            int id = 7;
            var customerData = GetStandardTestData();
            _customerRepository.Setup(p => p.GetById(id)).Returns(Task.FromResult(customerData.Where(c => c.Id == id).FirstOrDefault()));
            var result = await _customerService.GetById(id);
            Assert.True(result != null && result.Id == 7, "Single match with Id = 7 expected");
        }

        [Fact]
        public async void GetByIdNoMatch()
        {
            int id = 9;
            var customerData = GetStandardTestData();
            _customerRepository.Setup(p => p.GetById(id)).Returns(Task.FromResult(customerData.Where(c => c.Id == id).FirstOrDefault()));
            var result = await _customerService.GetById(id);
            Assert.Null(result);
        }

        [Fact]
        public async void Create()
        {
            CustomerDto newCustomer = new CustomerDto() { Id = 0, DateOfBirth = new DateTime(1983, 1, 1), FirstName = "Bill", LastName = "Bob" };
            _customerRepository.Setup(p => p.Create(newCustomer)).Returns(Task.FromResult(new CustomerDto() { Id = 8, DateOfBirth = new DateTime(1983, 1, 1), FirstName = "Bill", LastName = "Bob" }));
            var result = await _customerService.Create(newCustomer);
            Assert.True(result != null && result.Id == 8);

            // Id must be 0
            newCustomer = new CustomerDto() { Id = 8, DateOfBirth = new DateTime(1983, 1, 1), FirstName = "Bill", LastName = "Bob" };
            _customerRepository.Setup(p => p.Create(newCustomer)).Returns(Task.FromResult(new CustomerDto() { Id = 8, DateOfBirth = new DateTime(1983, 1, 1), FirstName = "Bill", LastName = "Bob" }));
            result = await _customerService.Create(newCustomer);
            Assert.Null(result);

            // First and Last name fields required
            newCustomer = new CustomerDto() { Id = 0, DateOfBirth = new DateTime(1983, 1, 1), FirstName = "", LastName = "" };
            _customerRepository.Setup(p => p.Create(newCustomer)).Returns(Task.FromResult(new CustomerDto() { Id = 8, DateOfBirth = new DateTime(1983, 1, 1), FirstName = "Bill", LastName = "Bob" }));
            result = await _customerService.Create(newCustomer);
            Assert.Null(result);
        }

        [Fact]
        public async void CreateValid()
        {
            CustomerDto newCustomer = new CustomerDto() { Id = 0, DateOfBirth = new DateTime(1983, 1, 1), FirstName = "Bill", LastName = "Bob" };
            _customerRepository.Setup(p => p.Create(newCustomer)).Returns(Task.FromResult(new CustomerDto() { Id = 8, DateOfBirth = new DateTime(1983, 1, 1), FirstName = "Bill", LastName = "Bob" }));
            var result = await _customerService.Create(newCustomer);
            Assert.True(result != null && result.Id == 8);
        }

        [Fact]
        public async void CreateIdNotZero()
        {
            // Id must be 0
            CustomerDto newCustomer = new CustomerDto() { Id = 8, DateOfBirth = new DateTime(1983, 1, 1), FirstName = "Bill", LastName = "Bob" };
            _customerRepository.Setup(p => p.Create(newCustomer)).Returns(Task.FromResult(new CustomerDto() { Id = 8, DateOfBirth = new DateTime(1983, 1, 1), FirstName = "Bill", LastName = "Bob" }));
            var result = await _customerService.Create(newCustomer);
            Assert.Null(result);
        }

        [Fact]
        public async void CreateMissingFields()
        {
            // First and Last name fields required
            var newCustomer = new CustomerDto() { Id = 0, DateOfBirth = new DateTime(1983, 1, 1), FirstName = "", LastName = "" };
            _customerRepository.Setup(p => p.Create(newCustomer)).Returns(Task.FromResult(new CustomerDto() { Id = 8, DateOfBirth = new DateTime(1983, 1, 1), FirstName = "", LastName = "" }));
            var result = await _customerService.Create(newCustomer);
            Assert.Null(result);
        }

        [Fact]
        public async void UpdateValidCustomer()
        {
            CustomerDto updatedCustomer = new CustomerDto() { Id = 1, DateOfBirth = new DateTime(1983, 1, 1), FirstName = "Alan", LastName = "Smith" };
            _customerRepository.Setup(p => p.GetById(1)).Returns(Task.FromResult(new CustomerDto() { Id = 8, DateOfBirth = new DateTime(1983, 1, 1), FirstName = "Fake", LastName = "Name" }));
            _customerRepository.Setup(p => p.Update(updatedCustomer)).Returns(Task.FromResult(true));
            var result = await _customerService.Update(updatedCustomer);
            Assert.True(result, "Update should occur");
        }

        [Fact]
        public async void UpdateMissingCustomerFields()
        {
            CustomerDto updatedCustomer = new CustomerDto() { Id = 1, DateOfBirth = new DateTime(1983, 1, 1), FirstName = "", LastName = "" };
            _customerRepository.Setup(p => p.GetById(1)).Returns(Task.FromResult(new CustomerDto() { Id = 8, DateOfBirth = new DateTime(1983, 1, 1), FirstName = "Fake", LastName = "Name" }));
            _customerRepository.Setup(p => p.Update(updatedCustomer)).Returns(Task.FromResult(true));
            var result = await _customerService.Update(updatedCustomer);
            Assert.False(result, "No update if missing fields");
        }

        [Fact]
        public async void UpdateInvalidCustomerId()
        {
            CustomerDto updatedCustomer = new CustomerDto() { Id = 8, DateOfBirth = new DateTime(1983, 1, 1), FirstName = "Bill", LastName = "Bob" };
            _customerRepository.Setup(p => p.GetById(8)).Returns(Task.FromResult(default(CustomerDto)));
            _customerRepository.Setup(p => p.Update(updatedCustomer)).Returns(Task.FromResult(true));
            var result = await _customerService.Update(updatedCustomer);
            Assert.False(result, "Customer with that Id does not exist cannot be updated");
        }

        [Fact]
        public async void DeleteValidCustomerId()
        {
            _customerRepository.Setup(p => p.Delete(7)).Returns(Task.FromResult(true));
            var result = await _customerService.Delete(7);
            Assert.True(result, "Repository returns true indicating delete occurred");
        }

        [Fact]
        public async void DeleteInvalidCustomerId()
        {
            _customerRepository.Setup(p => p.Delete(8)).Returns(Task.FromResult(false));
            var result = await _customerService.Delete(8);
            Assert.False(result, "Repository returns true indicating delete failed");
        }

        private List<CustomerDto> GetStandardTestData()
        {
            var customerData = new List<CustomerDto>();
            customerData.Add(new CustomerDto() { Id = 1, DateOfBirth = new DateTime(2000, 1, 1), FirstName = "Alan", LastName = "Smith" });
            customerData.Add(new CustomerDto() { Id = 2, DateOfBirth = new DateTime(1980, 1, 1), FirstName = "Anne", LastName = "Green" });
            customerData.Add(new CustomerDto() { Id = 3, DateOfBirth = new DateTime(1956, 1, 1), FirstName = "Bob", LastName = "Black" });
            customerData.Add(new CustomerDto() { Id = 4, DateOfBirth = new DateTime(1970, 1, 1), FirstName = "William", LastName = "Hargrave" });
            customerData.Add(new CustomerDto() { Id = 5, DateOfBirth = new DateTime(2005, 1, 1), FirstName = "Selina", LastName = "Kyle" });
            customerData.Add(new CustomerDto() { Id = 6, DateOfBirth = new DateTime(1978, 1, 1), FirstName = "Alice", LastName = "Hill" });
            customerData.Add(new CustomerDto() { Id = 7, DateOfBirth = new DateTime(1986, 1, 1), FirstName = "Heath", LastName = "Smith" });
            return customerData;
        }
    }
}