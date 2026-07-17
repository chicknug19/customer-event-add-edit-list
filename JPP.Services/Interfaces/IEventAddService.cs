using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JPP.Models.Event.Request;
using JPP.Models.Shared.Responses;

namespace JPP.Services.Interfaces
{
    public interface IEventAddService
    {
        Task<BaseResult<int>> AddEventAsync(EventRequest request);
    }
}
