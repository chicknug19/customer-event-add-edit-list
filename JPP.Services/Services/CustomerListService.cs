using System;
using System.Threading.Tasks;
using JPP.Data.Interfaces;
using JPP.Models.Customer.Responses;
using JPP.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace JPP.Services.Services
{
    public class CustomerListService : ICustomerListService
    {
        private readonly ICustomerListRepository _customerListRepository;
        private readonly ILogger<CustomerListService> _logger;

        public CustomerListService(
            ICustomerListRepository customerListRepository, 
            ILogger<CustomerListService> logger)
        {
            _customerListRepository = customerListRepository;
            _logger = logger;
        }

        public async Task<CustomerServiceResult> GetCustomerListAsync()
        {
            try
            {
                var data = await _customerListRepository.GetCustomerListAsync();
                
                return CustomerServiceResult.Ok(data, "Success");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching customer list");
                return CustomerServiceResult.Fail("An error occurred while fetching customers", 500);
            }
        }
    }
}