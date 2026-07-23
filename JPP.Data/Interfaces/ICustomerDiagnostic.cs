using JPP.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JPP.Models.Customer.Request;
using JPP.Models.Customer.Responses;

namespace JPP.Data.Interfaces
{
    public interface ICustomerDiagnosticRepository
    {
    Task<List<CustomerDiagnosticDto>> GetCustomerDiagnosticAsync(int CustomerId);
    Task<LatestCustomerEventDto> GetLatestCustomerEventAsync(int customerId);
    Task<int> AddCustomerDiagnosticAsync(NewCustomerDiagnosticDto dto);
    }
    
}