using System;
using JPP.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JPP.Models.Customer.Request;

namespace JPP.Data.Interfaces
{
    public interface ICustomerRepository
    {
        Task<bool> IdentityNoExistsAsync(string identityNo);

        Task<bool> EmailExistsAsync(string email);

        // Kunci 2: Pastikan tipe parameternya adalah CustomerRequest
        Task<int> CreateCustomerAsync(CustomerRequest request);
    }
}
