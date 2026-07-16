using JPP.Models.Customer.Responses;
using JPP.Models.Customer.Request;

namespace JPP.Services.Interfaces
{
    public interface ICustomerListService
    {
        Task<CustomerServiceResult> GetCustomerListAsync(CustomerListFilterRequest filter);
    }
}