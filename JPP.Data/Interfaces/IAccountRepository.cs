using JPP.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPP.Data.Interfaces
{
    public interface IAccountRepository
    {
        Task<BIZEmployee?> GetEmployeeByUsernameAsync(string username);
        Task<BIZSecurity?> GetEmployeeSecurityByEmployeeIdAsync(int employeeId);
        Task InsertUserSessionAsync(int userId, string? ipAddress, string? browser, string? browserVersion);
        Task<bool> EmailExistsAsync(string email);
        Task UpdatePasswordByEmailAsync(string email, string newPassword);
        Task SaveLoginAuditAsync(BIZEmployee employee);
    }
}
