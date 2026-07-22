using System;
using JPP.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JPP.Models.Customer.Request;

namespace JPP.Data.Interfaces
{
    public interface ICustomerAddRepository
    {
        Task<bool> EmailExistsAsync(string email);

        Task<int> CreateCustomerAsync(CustomerRequest request);
        Task<bool> PhoneNumberExistsAsync(string phoneNumber);
    }
}
