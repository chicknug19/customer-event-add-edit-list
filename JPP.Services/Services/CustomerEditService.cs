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
                    PhoneNumber2 = customer.PhoneNumber2,
                    EmailAddress = customer.EmailAddress,
                    Address1 = customer.Address1,
                    Address2 = customer.Address2,
                },
                IsReadOnly = false
            };
        }

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

            var isUpdated = await _customerEditRepository.UpdateCustomerAsync(request);

            if (!isUpdated)
            {
                return BaseResult<int>.Fail("Customer gagal diperbarui.", 400);
            }

            return BaseResult<int>.Ok(request.ID, "Customer berhasil diperbarui.", 200);
        }
    }
}