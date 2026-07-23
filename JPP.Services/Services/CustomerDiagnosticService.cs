using JPP.Data.Interfaces;
using JPP.Models.Customer.Responses;
using JPP.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JPP.Services.Services
{
    public class CustomerDiagnosticService : ICustomerDiagnosticService
    {
        private readonly ICustomerDiagnosticRepository _customerDiagnosticRepository;
        private readonly ILogger<CustomerDiagnosticService> _logger;

        public CustomerDiagnosticService(
            ICustomerDiagnosticRepository customerDiagnosticRepository,
            ILogger<CustomerDiagnosticService> logger)
        {
            _customerDiagnosticRepository = customerDiagnosticRepository;
            _logger = logger;
        }

        public async Task<List<CustomerDiagnosticDto>> GetCustomerDiagnosticAsync(int customerId)
        {
            try
            {
                return await _customerDiagnosticRepository.GetCustomerDiagnosticAsync(customerId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching customer diagnostics for CustomerId: {CustomerId}", customerId);
                return new List<CustomerDiagnosticDto>();
            }
        }
    }
}