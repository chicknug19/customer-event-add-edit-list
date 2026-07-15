using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JPP.Models.Customer.Responses.CustomerDto;
using JPP.Models.Customer.Responses;
using JPP.Models.Customer.Request;
using JPP.Models.Shared.Responses;
using System.Threading.Tasks;

namespace JPP.Services.Interfaces
{
    public interface ICustomerService
    {
        // Fungsi ini akan mengembalikan Response standar yang berisi ID Customer baru (int)
        Task<BaseResult<int>> AddCustomerAsync(CustomerRequest request);
    }
}
