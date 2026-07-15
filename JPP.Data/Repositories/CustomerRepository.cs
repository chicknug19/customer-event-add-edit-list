using Dapper;
using JPP.Data.DataAccess;
using JPP.Data.Interfaces;
using JPP.Models.Customer.Request;
using System;
using System.Threading.Tasks;

namespace JPP.Data.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ICrmDbConnectionFactory _crmDbConnectionFactory;

        public CustomerRepository(ICrmDbConnectionFactory crmDbConnectionFactory)
        {
            _crmDbConnectionFactory = crmDbConnectionFactory;
        }

        public async Task<bool> IdentityNoExistsAsync(string identityNo)
        {
            if (string.IsNullOrWhiteSpace(identityNo)) return false;

            const string sql = @"
                SELECT COUNT(1) 
                FROM BIZ_Customer 
                WHERE IdentityNo = @IdentityNo";

            using var conn = _crmDbConnectionFactory.Create();
            return await conn.ExecuteScalarAsync<int>(sql, new { IdentityNo = identityNo.Trim() }) > 0;
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

        // Ubah BIZCustomer menjadi CustomerRequest di baris ini 👇
        public async Task<int> CreateCustomerAsync(CustomerRequest request)
        {
            const string sql = @"
                INSERT INTO BIZ_Customer
                (
                    UID, DateCreated, LastUpdated, Inactive, Title, FirstName, MiddleName, LastName, 
                    IdentityNo, DOB, MaritalStatus, Gender, Race, Nation, Occupation, PhoneNumber, 
                    HPNum, EmailAddress, [Password], Password_WEB, BlockHouseNo, UnitNo, Address1, 
                    Address2, City, State, Country, Zip, CategoryID, StoreID, AcceptSMS, AcceptMailEmail
                )
                VALUES
                (
                    @UID, GETDATE(), GETDATE(), @Inactive, @Title, @FirstName, @MiddleName, @LastName, 
                    @IdentityNo, @DOB, @MaritalStatus, @Gender, @Race, @Nation, @Occupation, @PhoneNumber, 
                    @HPNum, @EmailAddress, @Password, @Password_WEB, @BlockHouseNo, @UnitNo, @Address1, 
                    @Address2, @City, @State, @Country, @Zip, @CategoryID, @StoreID, @AcceptSMS, @AcceptMailEmail
                );

                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using var conn = _crmDbConnectionFactory.Create();

            var newId = await conn.ExecuteScalarAsync<int>(sql, new
            {
                UID = Guid.NewGuid(),
                Inactive = false,
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
                AcceptMailEmail = request.AcceptMailEmail
            });

            return newId;
        }
    }
}