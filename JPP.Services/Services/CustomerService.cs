using JPP.Data.Interfaces;
using JPP.Models.Customer.Request;
using JPP.Models.Shared.Responses;
using JPP.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace JPP.Services.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<BaseResult<int>> AddCustomerAsync(CustomerRequest request)
        {
            try
            {
                // 1. Validasi IdentityNo
                if (!string.IsNullOrWhiteSpace(request.IdentityNo))
                {
                    bool isIdentityExist = await _customerRepository.IdentityNoExistsAsync(request.IdentityNo);
                    if (isIdentityExist)
                    {
                        return BaseResult<int>.Fail("Identity Number sudah terdaftar di sistem.", 400);
                    }
                }

                // 2. Validasi Email Address
                if (!string.IsNullOrWhiteSpace(request.EmailAddress))
                {
                    bool isEmailExist = await _customerRepository.EmailExistsAsync(request.EmailAddress);
                    if (isEmailExist)
                    {
                        return BaseResult<int>.Fail("Email Address sudah digunakan oleh customer lain.", 400);
                    }
                }

                // 3. Simpan data ke Database
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