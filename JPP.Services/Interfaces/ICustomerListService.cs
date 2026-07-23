using JPP.Models.Customer.Request;
using JPP.Models.Customer.Responses;
using JPP.Models.CustomerEvent.Request;

namespace JPP.Services.Interfaces
{
    public interface ICustomerListService
    {
        Task<CustomerServiceResult> GetCustomerListAsync(CustomerListFilterRequest filter);
        Task<bool> SaveCustomerEventsAsync(CustomerEventSaveRequest request);
    }
}