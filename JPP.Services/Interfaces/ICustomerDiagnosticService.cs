using JPP.Models.Customer.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JPP.Services.Interfaces
{
    public interface ICustomerDiagnosticService
    {
        Task<List<CustomerDiagnosticDto>> GetCustomerDiagnosticAsync(int customerId);
        Task<bool> AddCustomerDiagnosticAsync(NewCustomerDiagnosticDto request);
    }
}