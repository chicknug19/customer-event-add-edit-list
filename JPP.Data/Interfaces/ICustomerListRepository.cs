using JPP.Data.Entities;
using JPP.Models.Customer.Request;
using JPP.Models.Customer.Responses;
using JPP.Models.CustomerEvent.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPP.Data.Interfaces
{
    public interface ICustomerListRepository
    {
    Task<List<CustomerListDto>> GetCustomerListAsync(CustomerListFilterRequest filter);
    Task<bool> SaveCustomerEventsAsync(CustomerEventSaveRequest request);
    }
    
}