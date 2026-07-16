using JPP.Data.Interfaces;
using JPP.Models.Customer.Request;
using JPP.Models.Shared.Responses;
using JPP.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace JPP.Services.Services
{
    public class CustomerAddService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerAddService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<BaseResult<int>> AddCustomerAsync(CustomerRequest request)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(request.EmailAddress))
                {
                    bool isEmailExist = await _customerRepository.EmailExistsAsync(request.EmailAddress);
                    if (isEmailExist)
                    {
                        return BaseResult<int>.Fail("Email sudah terdaftar.", 400);
                    }
                }

                var newId = await _customerRepository.CreateCustomerAsync(request);

                return BaseResult<int>.Ok(newId, "Customer berhasil ditambahkan.", 200);
            }
            catch (Exception ex)
            {
                return BaseResult<int>.Fail($"Terjadi kesalahan sistem: {ex.Message}", 500);
            }
        }
    }
}