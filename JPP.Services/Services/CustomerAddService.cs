using JPP.Data.Interfaces;
using JPP.Models.Customer.Request;
using JPP.Models.Shared.Responses;
using JPP.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace JPP.Services.Services
{
    public class CustomerAddService : ICustomerAddService
    {
        private readonly ICustomerAddRepository _customerRepository;

        public CustomerAddService(ICustomerAddRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }


        public async Task<BaseResult<int>> AddCustomerAsync(CustomerRequest request)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(request.PhoneNumber) && request.PhoneNumber.StartsWith("0"))
                {
                    request.PhoneNumber = request.PhoneNumber.Substring(1);
                }

                if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
                {
                    bool isPhoneExist = await _customerRepository.PhoneNumberExistsAsync(request.PhoneNumber);
                    if (isPhoneExist)
                    {
                        return BaseResult<int>.Fail($"Phone Number '{request.PhoneNumber}' Already registered. Please use another number.", 400);
                    }
                }

                if (request.StoreId.HasValue)
                {
                    request.AccountNumber = await _customerRepository.GenerateAccountNumberAsync(request.StoreId.Value);
                }
                else
                {
                    return BaseResult<int>.Fail("A store must be selected.", 400);
                }

                var newId = await _customerRepository.CreateCustomerAsync(request);
                return BaseResult<int>.Ok(newId, "Customer successfully added.", 200);
                //if (!string.IsNullOrWhiteSpace(request.EmailAddress))
                //{
                //    bool isEmailExist = await _customerRepository.EmailExistsAsync(request.EmailAddress);
                //    if (isEmailExist)
                //    {
                //        return BaseResult<int>.Fail("The email is already registered.", 400);
                //    }
                //}

            }
            catch (Exception ex)
            {
                return BaseResult<int>.Fail($"A system error occurred: {ex.Message}", 500);
            }
        }

        public async Task<string> GetPreviewAccountNumberAsync(int storeId)
        {
            return await _customerRepository.GenerateAccountNumberAsync(storeId);
        }





    }
}