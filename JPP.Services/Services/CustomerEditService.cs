using JPP.Data.Interfaces;
using JPP.Models.Customer.Request;
using JPP.Models.Customer.Responses;
using JPP.Models.Shared.Responses;
using JPP.Services.Interfaces;

namespace JPP.Services.Services
{
    public class CustomerEditService : ICustomerEditService
    {
        private readonly ICustomerEditRepository _customerEditRepository;

        public CustomerEditService(ICustomerEditRepository customerEditRepository)
        {
            _customerEditRepository = customerEditRepository;
        }

        public async Task<CustomerDetailViewModel?> BuildEditViewModelAsync(int id)
        {
            if (id <= 0)
            {
                return null;
            }

            var customer = await _customerEditRepository.GetCustomerByIdAsync(id);

            if (customer == null)
            {
                return null;
            }

            return new CustomerDetailViewModel
            {
                Form = new CustomerRequest
                {
                    ID = customer.CustomerId,
                    FirstName = customer.FirstName,
                    MiddleName = customer.MiddleName,
                    LastName = customer.LastName,
                    PhoneNumber = customer.PhoneNumber,
                    EmailAddress = customer.EmailAddress,
                    Address1 = customer.Address1,
                    EventId = customer.EventId,
                    EventName = customer.EventName,
                    StoreId = customer.StoreId,
                    Age = customer.Age,
                    AccountNumber = customer.AccountNumber,
                    District = customer.District
                },
                EventName = customer.EventName ?? string.Empty,
                IsReadOnly = false
            };
        }

        // Save
        public async Task<BaseResult<int>> SaveCustomerAsync(CustomerRequest request)
        {
            if (request == null)
            {
                return BaseResult<int>.Fail("Data customer tidak valid.", 400);
            }

            if (request.ID <= 0)
            {
                return BaseResult<int>.Fail("Customer ID tidak valid.", 400);
            }

            if (!string.IsNullOrWhiteSpace(request.PhoneNumber) && request.PhoneNumber.StartsWith("0"))
            {
                request.PhoneNumber = request.PhoneNumber.Substring(1);
            }

            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                bool isPhoneExist = await _customerEditRepository.PhoneNumberExistsAsync(request.PhoneNumber, request.ID);
                if (isPhoneExist)
                {
                    return BaseResult<int>.Fail($"Phone Number '{request.PhoneNumber}' Already registered. Please use another number.", 400);
                }
            }

            var isUpdated = await _customerEditRepository.UpdateCustomerAsync(request);

            if (!isUpdated)
            {
                return BaseResult<int>.Fail("Customer gagal diperbarui.", 400);
            }

            return BaseResult<int>.Ok(request.ID, "Customer berhasil diperbarui.", 200);
        }
    }
}