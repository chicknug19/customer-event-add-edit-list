using Dapper;
using JPP.Data.DataAccess;
using JPP.Data.Interfaces;
using JPP.Models.Customer.Request;
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
                (
                    UID, DateCreated, LastUpdated, Inactive, 
                    FirstName, MiddleName, LastName, 
                    PhoneNumber, PhoneNumber2, EmailAddress, Address1, Address2, EventId /* Tambahkan ini */
                )
                VALUES
                (
                    NEWID(), GETDATE(), GETDATE(), 0, 
                    @FirstName, @MiddleName, @LastName, 
                    @PhoneNumber, @PhoneNumber2, @EmailAddress, @Address1, @Address2, @EventId /* Tambahkan ini */
                );";

            using var conn = _crmDbConnectionFactory.Create();

            var newId = await conn.ExecuteScalarAsync<int>(sql, new
            {
                FirstName = request.FirstName?.Trim() ?? string.Empty,
                MiddleName = request.MiddleName?.Trim(),
                LastName = request.LastName?.Trim(),
                PhoneNumber = request.PhoneNumber?.Trim() ?? string.Empty,
                PhoneNumber2 = request.PhoneNumber2?.Trim(),
                EmailAddress = request.EmailAddress?.Trim(),
                Address1 = request.Address1?.Trim() ?? string.Empty,
                Address2 = request.Address2?.Trim(),
                EventId = request.EventId
            });

            return newId;
        }
    }
}