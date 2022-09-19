using Customer.Contracts;
using Customer.Services;
using Microsoft.AspNetCore.Mvc;


namespace CustomerAPI.Controllers
{
    /// <summary>
    /// Controller for creating, editing, reading and deleting Customer records
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ILogger<CustomerController> _logger;
        private readonly ICustomerService _customerService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">ILogger CustomerController</param>
        /// <param name="customerService">ICustomerService</param>
        public CustomerController(ILogger<CustomerController> logger, ICustomerService customerService)
        {
            _logger = logger;
            _customerService = customerService;
        }

        /// <summary>
        /// Gets the customer(s) whose first or last name contain the search term
        /// </summary>
        /// <param name="searchTerm">The term used to search for a customer match</param>
        /// <returns>A list of CustomerDto</returns>
        [HttpGet("GetByName/{searchTerm}")]
        [ProducesResponseType(typeof(IEnumerable<CustomerDto>), 200)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetByName(string searchTerm)
        {
            try
            {
                return await _customerService.GetByName(searchTerm);
            }
            catch(Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Gets a single Customer record that matches the Id
        /// </summary>
        /// <param name="id">The Id of the customer</param>
        /// <returns>A CustomerDto</returns>
        [HttpGet("GetById/{id}")]
        [ProducesResponseType(typeof(CustomerDto), 200)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CustomerDto>> GetById(int id)
        {
            try
            {
                var customer = await _customerService.GetById(id);
                if (customer == null)
                {
                    return StatusCode(400);
                }
                return customer;
            }
            catch(Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Creates a new customer enter
        /// </summary>
        /// <param name="newRecord">CustomerDto representing new record. Id should be 0</param>
        /// <returns>CustomerDto with Id set</returns>
        [HttpPost("Create")]
        [ProducesResponseType(typeof(CustomerDto), 200)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CustomerDto>> Create([FromBody] CustomerDto newRecord)
        {
            try
            {
                var createdObject = await _customerService.Create(newRecord);
                if (createdObject == null)
                {
                    return StatusCode(400);
                }
                return createdObject;
            }
            catch(Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Updates an existing Customer record
        /// </summary>
        /// <param name="updatedRecord">CustomerDto with updated information </param>
        /// <returns>Bool indicating success or failure of update</returns>
        [HttpPut("Update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> Update([FromBody] CustomerDto updatedRecord)
        {
            try
            {
                bool recordUpdated = await _customerService.Update(updatedRecord);
                return recordUpdated ? NoContent() : StatusCode(400);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Deletes a Customer record
        /// </summary>
        /// <param name="id">The Id of the record to be deleted</param>
        /// <returns>Bool indicating success or failure of the deletion</returns>
        [HttpDelete("Delete/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                bool recordDeleted = await _customerService.Delete(id);
                return recordDeleted ?  NoContent() : StatusCode(400);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode(500);
            }
        }
    }
}