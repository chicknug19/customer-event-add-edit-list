using Dapper;
using JPP.Data.DataAccess;
using JPP.Data.Interfaces;
using JPP.Models.Customer.Request;
using JPP.Models.Customer.Responses.CustomerDto;

namespace JPP.Data.Repositories
{
    public class CustomerEditRepository : ICustomerEditRepository
    {
        private readonly ICrmDbConnectionFactory _crmDbConnectionFactory;

        public CustomerEditRepository(ICrmDbConnectionFactory crmDbConnectionFactory)
        {
            _crmDbConnectionFactory = crmDbConnectionFactory;
        }

        //save
        public async Task<CustomerDto?> GetCustomerByIdAsync(int id)
        {
            if (id <= 0)
            {
                return null;
            }

            const string sql = @"
                SELECT
                    c.ID AS CustomerId,
                    c.FirstName,
                    c.MiddleName,
                    c.LastName,
                    c.PhoneNumber,
                    c.PhoneNumber2,
                    c.EmailAddress,
                    c.Address1,
                    c.Address2,
                    c.EventID AS EventId,
                    e.Name AS EventName,
                    c.StoreID AS StoreId,
                    c.AccountNumber,
                    c.Age,
                    c.District
                FROM BIZ_Customer c
                LEFT JOIN BIZ_Event e ON e.Id = c.EventID
                WHERE c.ID = @Id";

            using var conn = _crmDbConnectionFactory.Create();
            return await conn.QuerySingleOrDefaultAsync<CustomerDto>(sql, new { Id = id });
        }

        public async Task<bool> UpdateCustomerAsync(CustomerRequest request)
        {
            if (request == null || request.ID <= 0)
            {
                return false;
            }

            const string sql = @"
                UPDATE BIZ_Customer
                SET
                    FirstName = @FirstName,
                    MiddleName = @MiddleName,
                    LastName = @LastName,
                    PhoneNumber = @PhoneNumber,
                    EmailAddress = @EmailAddress,
                    Address1 = @Address1,
                    StoreID = @StoreId,
                    Age = @Age,
                    AccountNumber = @AccountNumber,
                    District = @District,
                    LastUpdated = GETDATE()
                WHERE ID = @ID";

            using var conn = _crmDbConnectionFactory.Create();
            var rowsAffected = await conn.ExecuteAsync(sql, new
            {
                ID = request.ID,
                FirstName = request.FirstName?.Trim() ?? string.Empty,
                MiddleName = request.MiddleName?.Trim(),
                LastName = request.LastName?.Trim(),
                PhoneNumber = request.PhoneNumber?.Trim() ?? string.Empty,
                EmailAddress = request.EmailAddress?.Trim(),
                Address1 = request.Address1?.Trim() ?? string.Empty,
                StoreId = request.StoreId,
                Age = request.Age,
                AccountNumber = request.AccountNumber,
                District = request.District?.Trim()
            });

            return rowsAffected > 0;
        }
        
        public async Task<bool> PhoneNumberExistsAsync(string phoneNumber, int? excludeCustomerId = null)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber)) return false;

            const string sql = @"
                SELECT COUNT(1)
                FROM BIZ_Customer
                WHERE PhoneNumber = @PhoneNumber
                AND (@ExcludeCustomerId IS NULL OR ID <> @ExcludeCustomerId)";

            using var conn = _crmDbConnectionFactory.Create();
            return await conn.ExecuteScalarAsync<int>(sql, new
            {
                PhoneNumber = phoneNumber.Trim(),
                ExcludeCustomerId = excludeCustomerId
            }) > 0;
        }
    }
}