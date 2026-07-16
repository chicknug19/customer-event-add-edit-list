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

        public async Task<CustomerDto?> GetCustomerByIdAsync(int id)
        {
            if (id <= 0)
            {
                return null;
            }

            const string sql = @"
                SELECT
                    ID AS CustomerId,
                    FirstName,
                    MiddleName,
                    LastName,
                    PhoneNumber,
                    PhoneNumber2,
                    EmailAddress,
                    Address1,
                    Address2
                FROM BIZ_Customer
                WHERE ID = @Id";

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
                    PhoneNumber2 = @PhoneNumber2,
                    EmailAddress = @EmailAddress,
                    Address1 = @Address1,
                    Address2 = @Address2,
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
                PhoneNumber2 = request.PhoneNumber2?.Trim(),
                EmailAddress = request.EmailAddress?.Trim(),
                Address1 = request.Address1?.Trim() ?? string.Empty,
                Address2 = request.Address2?.Trim()
            });

            return rowsAffected > 0;
        }
    }
}