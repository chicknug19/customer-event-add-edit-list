using System;
using JPP.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPP.Data.Interfaces
{
    public interface ICustomerRepository
    {
        Task<bool> IdentityNoExistsAsync(string identityNo);
        Task<bool> EmailExistsAsync(string email);
        Task<int> CreateCustomerAsync(BIZCustomer customer);
    }
}
