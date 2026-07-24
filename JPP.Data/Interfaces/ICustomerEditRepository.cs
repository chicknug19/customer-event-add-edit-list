using JPP.Models.Customer.Request;
using JPP.Models.Customer.Responses.CustomerDto;

namespace JPP.Data.Interfaces
{
    public interface ICustomerEditRepository
    {
        Task<CustomerDto?> GetCustomerByIdAsync(int id);
        Task<bool> UpdateCustomerAsync(CustomerRequest request);
        Task<bool> PhoneNumberExistsAsync(string phoneNumber, int? excludeCustomerId = null);
        // save
    }
}