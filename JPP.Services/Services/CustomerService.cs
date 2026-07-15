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
                // 1. Validasi
                if (!string.IsNullOrWhiteSpace(request.IdentityNo))
                {
                    bool isKtpExist = await _customerRepository.IdentityNoExistsAsync(request.IdentityNo);
                    if (isKtpExist)
                    {
                        return BaseResult<int>.Fail("Nomor Identitas (KTP) sudah terdaftar.", 400);
                    }
                }

                if (!string.IsNullOrWhiteSpace(request.EmailAddress))
                {
                    bool isEmailExist = await _customerRepository.EmailExistsAsync(request.EmailAddress);
                    if (isEmailExist)
                    {
                        return BaseResult<int>.Fail("Email sudah terdaftar.", 400);
                    }
                }

                // 2. Langsung lempar request ke Repository
                var newId = await _customerRepository.CreateCustomerAsync(request);

                // 3. Kembalikan Response
                return BaseResult<int>.Ok(newId, "Customer berhasil ditambahkan.", 200);
            }
            catch (Exception ex)
            {
                return BaseResult<int>.Fail($"Terjadi kesalahan sistem: {ex.Message}", 500);
            }
        }
    }
}