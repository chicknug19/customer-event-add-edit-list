using JPP.Models.Customer.Request;
using JPP.Models.Customer.Responses;
using JPP.Models.Shared.Responses;

namespace JPP.Services.Interfaces
{
    public interface ICustomerEditService
    {
        Task<CustomerDetailViewModel?> BuildEditViewModelAsync(int id);
        Task<BaseResult<int>> SaveCustomerAsync(CustomerRequest request);
    }
}