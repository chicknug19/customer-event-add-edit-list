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
            using var conn = _crmDbConnectionFactory.Create();

            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            using var transaction = conn.BeginTransaction();

            try
            {
                const string sqlCustomer = @"
                INSERT INTO BIZ_Customer 
                (FirstName, MiddleName, LastName, PhoneNumber, EmailAddress, Address1, Age, EventId, StoreId, AccountNumber, District)
                VALUES 
                (@FirstName, @MiddleName, @LastName, @PhoneNumber, @EmailAddress, @Address1, @Age, @EventId, @StoreId, @AccountNumber, @District);
                
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

                var newCustomerId = await conn.ExecuteScalarAsync<int>(sqlCustomer, new
                {
                    FirstName = request.FirstName?.Trim() ?? string.Empty,
                    MiddleName = request.MiddleName?.Trim(),
                    LastName = request.LastName?.Trim(),
                    PhoneNumber = request.PhoneNumber?.Trim() ?? string.Empty,
                    EmailAddress = request.EmailAddress?.Trim() ?? string.Empty,
                    Address1 = request.Address1?.Trim() ?? string.Empty,
                    Age = request.Age.Value,
                    EventId = request.EventId,
                    StoreId = request.StoreId,
                    AccountNumber = request.AccountNumber,
                    District = request.District
                }, transaction);

                if (request.EventId.HasValue && request.EventId.Value > 0)
                {
                    const string sqlCustomerEvent = @"
                    INSERT INTO Customer_Event 
                    (UID, CustomerID, EventID, HQID)
                    VALUES 
                    (@UID, @CustomerID, @EventID, @HQID);";

                    await conn.ExecuteAsync(sqlCustomerEvent, new
                    {
                        UID = Guid.NewGuid().ToString(),
                        CustomerID = newCustomerId,
                        EventID = request.EventId.Value, 
                        HQID = request.StoreId  
                    }, transaction);
                }

                transaction.Commit();

                return newCustomerId;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
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
                "CRM_GetNewCustNoBaru",
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