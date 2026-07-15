using JPP.Models.Customer.Responses;
using JPP.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace JPP.Services.Services
{
    public class CustomerListService : ICustomerListService
    {
        private readonly ILogger<CustomerService> _logger;

        public CustomerListService(ILogger<CustomerService> logger)
        {
            _logger = logger;
        }

        public async Task<CustomerServiceResult> GetCustomerListAsync()
        {
            try
            {
                
                // var customers = await _customerRepository.GetAllAsync();
                // var customerListDtos = customers.Select(c => new CustomerListDto
                // {
                //     CustomerID = c.CustomerID,
                //     FullName = $"{c.FirstName} {c.LastName}".Trim(),
                //     Address1 = c.Address1,
                //     PhoneNumber = c.PhoneNumber
                // }).ToList();

                var mockData = new List<CustomerListDto>();
                return CustomerServiceResult.Ok(mockData, "Success");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching customer list");
                return CustomerServiceResult.Fail("An error occurred while fetching customers", 500);
            }
        }
    }
}