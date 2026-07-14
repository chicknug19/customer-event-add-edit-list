using Dapper;
using JPP.Data.DataAccess;
using JPP.Data.Entities;
using JPP.Data.Interfaces;
using Microsoft.Extensions.Logging;
using System.Data;

namespace JPP.Data.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ICrmDbConnectionFactory _db;
        private readonly ILogger<AccountRepository> _logger;

        public AccountRepository(
            ICrmDbConnectionFactory db,
            ILogger<AccountRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<BIZEmployee?> GetEmployeeByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return null;
            }

            const string sql = @"
            SELECT TOP 1
                E.ID AS Id,
                ISNULL(E.Username, '') AS Username,
                ISNULL(E.[Password], '') AS [Password],
                ISNULL(E.EmailAddress, '') AS EmailAddress,
                ISNULL(E.DisplayName, '') AS DisplayName,
                ISNULL(E.HQID, 0) AS HqId,
                ISNULL(E.StoreID, 0) AS StoreId,
                E.RoleID AS RoleID,
                CASE WHEN E.RoleID = 0 THEN 'Admin' ElSE Z.RoleName END AS RoleName
            FROM BIZ_Employee E JOIN BIZ_EmployeeDepartment ED ON E.EmployeeDepartmentID = ED.ID
            OUTER APPLY 
            (
                SELECT TOP 1 R.RoleName FROM BIZ_EmployeeRoles R WHERE R.ID = E.RoleID
            ) Z
            WHERE E.IsDeleted = 0
              AND ISNULL(E.Inactive, 0) = 0
              AND Username = @Username";

            using var conn = _db.Create();

            return await conn.QuerySingleOrDefaultAsync<BIZEmployee>(
                sql,
                new
                {
                    Username = username.Trim()
                });
        }

        public async Task<BIZSecurity?> GetEmployeeSecurityByEmployeeIdAsync(int employeeId)
        {
            const string sql = @"
            SELECT TOP 1
                EmployeeID AS EmployeeId,
                ISNULL(modHome, 0) AS ModHome
            FROM BIZ_Security
            WHERE EmployeeID = @EmployeeID";

            using var conn = _db.Create();

            return await conn.QuerySingleOrDefaultAsync<BIZSecurity>(
                sql,
                new
                {
                    EmployeeID = employeeId
                });
        }

        public async Task InsertUserSessionAsync(
            int userId,
            string? ipAddress,
            string? browser,
            string? browserVersion)
        {
            const string sql = @"
            INSERT INTO BIZ_UserSession
            (
                UserId,
                Login,
                IPAddress,
                Browser,
                Browser_Version
            )
            VALUES
            (
                @UserId,
                GETDATE(),
                @IPAddress,
                @Browser,
                @BrowserVersion
            )";

            using var conn = _db.Create();

            await conn.ExecuteAsync(
                sql,
                new
                {
                    UserId = userId,
                    IPAddress = ipAddress,
                    Browser = browser,
                    BrowserVersion = browserVersion
                });
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            const string sql = @"
            SELECT COUNT(ID)
            FROM BIZ_Employee
            WHERE IsDeleted = 0
              AND EmailAddress = @EmailAddress";

            using var conn = _db.Create();

            var total = await conn.ExecuteScalarAsync<int>(
                sql,
                new
                {
                    EmailAddress = email.Trim()
                });

            return total > 0;
        }

        public async Task UpdatePasswordByEmailAsync(string email, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return;
            }

            const string sql = @"
            UPDATE BIZ_Employee
            SET
                [Password] = @Password,
                LastUpdated = GETDATE()
            WHERE ISNULL(Inactive, 0) = 0
              AND EmailAddress = @EmailAddress";

            using var conn = _db.Create();

            await conn.ExecuteAsync(
                sql,
                new
                {
                    Password = newPassword,
                    EmailAddress = email.Trim()
                });
        }

        public Task SaveLoginAuditAsync(BIZEmployee employee)
        {
            _logger.LogInformation(
                "Login audit placeholder. UserId: {UserId}, DisplayName: {DisplayName}",
                employee.Id,
                employee.DisplayName);

            return Task.CompletedTask;
        }
    }
}