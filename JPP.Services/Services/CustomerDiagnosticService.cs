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

        public async Task<bool> AddCustomerDiagnosticAsync(NewCustomerDiagnosticDto request)
        {
            try
            {
                var latestEvent = await _customerDiagnosticRepository.GetLatestCustomerEventAsync(request.CustomerId);

                if (latestEvent == null)
                {
                    _logger.LogWarning("No Customer_Event found for CustomerId: {CustomerId}", request.CustomerId);
                    return false;
                }

                request.HQID = latestEvent.HQID;
                request.EventId = latestEvent.EventId;

                var newId = await _customerDiagnosticRepository.AddCustomerDiagnosticAsync(request);
                return newId > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving customer diagnostic for CustomerId: {CustomerId}", request.CustomerId);
                return false;
            }
        }
    }
}