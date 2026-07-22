using Dapper;
using JPP.Data.DataAccess;
using JPP.Data.Interfaces;
using JPP.Models.Customer.Request;
using System.Data;
using System;
using System.Threading.Tasks;

namespace JPP.Data.Repositories
{
    public class CustomerRepository : ICustomerAddRepository
    {
        private readonly ICrmDbConnectionFactory _crmDbConnectionFactory;

        public CustomerRepository(ICrmDbConnectionFactory crmDbConnectionFactory)
        {
            _crmDbConnectionFactory = crmDbConnectionFactory;
        }


        public async Task<bool> EmailExistsAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            const string sql = @"
                SELECT COUNT(1) 
                FROM BIZ_Customer 
                WHERE EmailAddress = @EmailAddress";

            using var conn = _crmDbConnectionFactory.Create();
            return await conn.ExecuteScalarAsync<int>(sql, new { EmailAddress = email.Trim() }) > 0;
        }

        public async Task<int> CreateCustomerAsync(CustomerRequest request)
        {
            const string sql = @"
            INSERT INTO BIZ_Customer 
            (FirstName, MiddleName, LastName, PhoneNumber, EmailAddress, Address1, EventId, StoreId, AccountNumber)
            VALUES 
            (@FirstName, @MiddleName, @LastName, @PhoneNumber, @EmailAddress, @Address1, @EventId, @StoreId, @AccountNumber);
    
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using var conn = _crmDbConnectionFactory.Create();

            var newId = await conn.ExecuteScalarAsync<int>(sql, new
            {
                FirstName = request.FirstName?.Trim() ?? string.Empty,
                MiddleName = request.MiddleName?.Trim(),
                LastName = request.LastName?.Trim(),
                PhoneNumber = request.PhoneNumber?.Trim() ?? string.Empty,
                EmailAddress = request.EmailAddress?.Trim() ?? string.Empty,
                Address1 = request.Address1?.Trim() ?? string.Empty,
                EventId = request.EventId,
                StoreId = request.StoreId,

                AccountNumber = request.AccountNumber
            });

            return newId;
        }


        public async Task<bool> PhoneNumberExistsAsync(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber)) return false;

            const string sql = "SELECT COUNT(1) FROM BIZ_Customer WHERE PhoneNumber = @PhoneNumber";

            using var conn = _crmDbConnectionFactory.Create();
            return await conn.ExecuteScalarAsync<int>(sql, new { PhoneNumber = phoneNumber.Trim() }) > 0;
        }


        public async Task<string> GenerateAccountNumberAsync(int storeId)
        {
            using var conn = _crmDbConnectionFactory.Create();

            // Memanggil Stored Procedure bawaan database
            var accNo = await conn.QuerySingleOrDefaultAsync<string>(
                "CRM_GetNewCustNo",
                new { strStoreID = storeId.ToString() },
                commandType: CommandType.StoredProcedure);

            if (!string.IsNullOrEmpty(accNo))
            {
                int digitIndex = accNo.IndexOfAny("0123456789".ToCharArray());
                if (digitIndex >= 0)
                {
                    accNo = accNo.Insert(digitIndex, "E");
                }
            }

            return accNo ?? string.Empty;
        }


    }
}