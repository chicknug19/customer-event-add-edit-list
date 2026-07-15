using JPP.Data.Interfaces;
using JPP.Data.Entities;
using JPP.Models.Customer.Responses.CustomerDto;
using JPP.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using JPP.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using JPP.Models.Customer.Request;
using JPP.Models.Shared.Responses;

namespace JPP.Services.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        // Inject CustomerRepository ke dalam Service
        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<BaseResult<int>> AddCustomerAsync(CustomerRequest request)
        {
            try
            {
                // ==========================================
                // 1. VALIDASI DATA VIA REPOSITORY
                // ==========================================
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

                // ==========================================
                // 2. MAPPING DTO/REQUEST KE ENTITY
                // ==========================================
                var newCustomer = new BIZCustomer
                {
                    Title = request.Title,
                    FirstName = request.FirstName?.Trim() ?? string.Empty,
                    MiddleName = request.MiddleName?.Trim(),
                    LastName = request.LastName?.Trim(),
                    IdentityNo = request.IdentityNo?.Trim() ?? string.Empty,
                    DOB = request.DOB,
                    MaritalStatus = request.MaritalStatus,
                    Gender = request.Gender,
                    Race = request.Race,
                    Nation = request.Nation,
                    Occupation = request.Occupation,

                    PhoneNumber = request.PhoneNumber,
                    HPNum = request.HPNum,
                    EmailAddress = request.EmailAddress?.Trim(),
                    Password = request.Password ?? string.Empty,
                    Password_WEB = request.Password_WEB,

                    BlockHouseNo = request.BlockHouseNo,
                    UnitNo = request.UnitNo,
                    Address1 = request.Address1,
                    Address2 = request.Address2,
                    City = request.City,
                    State = request.State,
                    Country = request.Country,
                    Zip = request.Zip,

                    CategoryID = request.CategoryID,
                    StoreID = request.StoreID,
                    AcceptSMS = request.AcceptSMS,
                    AcceptMailEmail = request.AcceptMailEmail,

                    // Default values
                    DateCreated = DateTime.Now,
                    LastUpdated = DateTime.Now,
                    Inactive = false,
                    UID = Guid.NewGuid()
                };

                // ==========================================
                // 3. SIMPAN KE DATABASE VIA REPOSITORY
                // ==========================================
                var newId = await _customerRepository.CreateCustomerAsync(newCustomer);

                // ==========================================
                // 4. KEMBALIKAN RESPONSE SUKSES
                // ==========================================
                return BaseResult<int>.Ok(newId, "Customer berhasil ditambahkan.", 200);
            }
            catch (Exception ex)
            {
                // Jika error, return status 500 beserta pesannya
                return BaseResult<int>.Fail($"Terjadi kesalahan sistem: {ex.Message}", 500);
            }
        }
    }
}